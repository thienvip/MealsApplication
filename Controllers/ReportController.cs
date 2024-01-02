using MealApplication.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Caching.Memory;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using NuGet.Packaging;
using NuGet.Protocol;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RestSharp.Extensions;
using src.Core.Data;
using src.Core.Domains;
using src.Repositories.Category;
using src.Repositories.Students;
using src.Web.Common;
using src.Web.Common.Models.ExportTypeModels;
using System.Drawing;
using System.Drawing.Printing;
using static src.Core.Data.MealPagedDataRequest;

namespace MealApplication.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IBaseCategoryRepository _baseCategory;
        private readonly IUserSession _userSession;
        private readonly IStudentRepository _studentRepository;
        private IWebHostEnvironment _hostingEnvironment;
        private string _fileName;
        private string _filePath;
        private IMemoryCache _cache;

        public ReportController(IBaseCategoryRepository baseCategory, IUserSession userSession, IStudentRepository studentRepository, IWebHostEnvironment hostingEnvironment, IMemoryCache cache)
        {
            _baseCategory = baseCategory;
            _userSession = userSession;
            _studentRepository = studentRepository;
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
            _filePath = _hostingEnvironment.WebRootPath + "/HandBookConfirmation/";

        }
        public async Task<IActionResult> Index()
        {
            var entities = await _baseCategory.getCampusByUser(_userSession.Id);
            var listStudent = new List<Student>();
          
            for (int i = 0; i < entities.Count(); i++)
            {
                var students = await _studentRepository.GetAllStudentsByCampusCode(entities[i].code);
                if (students.Count() > 0)
                {
                    listStudent.AddRange(students);
                }
            }
            return View(entities);
        }

        public  IActionResult GetStudentsByCampus(MealPagedDataRequest data)
        {

            int filteredResultsCount;
            int totalResultsCount;
            var students = new List<Student>();

            var res = studentSearch(data, out filteredResultsCount, out totalResultsCount);

            if (data.campusCode?.Length == 0 || data.campusCode == null)
            return Json(new { success = false, data = students });       

            foreach (var student in res)
            {
                students.Add(student);
            }

            _cache.Set("students", students);

            return Json(new
            {
                draw = data.draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = students,
            });


        }

        public IList<Student> studentSearch (MealPagedDataRequest model, out int filteredResultsCount, out int totalResultsCount)
        {
            var searchBy = (model.search != null) ? model.search.value : null;
            var take = model.length;
            var skip = model.start;

            string sortBy = "";
            bool sortDir = true;

            if (model.order != null)
            {               
                sortBy = model.columns[model.order[0].column].data;
                sortDir = model.order[0].dir.ToLower() == "desc";
            }

            // search the dbase taking into consideration table sorting and paging
            var result =  _studentRepository.GetPagedAllStudentsByCampusCode(model,searchBy, take, skip, sortBy, sortDir, out filteredResultsCount, out totalResultsCount); ;
            if (result == null)
            {
                return new List<Student>();
            }
            return result;
        }



        public IActionResult ExportToExcel([FromBody] ExportTypeModels data)
        {
                var type = data.Type;

                var cachedStudents = _cache.Get("students") as List<Student>;
                List<Student>? listStudent = null;

                listStudent = cachedStudents?.Select(s => new Student
                {
                    StudentCode = s.StudentCode,
                    StudentName = s.StudentName,
                    CampusCode = s.CampusCode,
                    CampusName = s.CampusName,
                    DateOfBirth = s.DateOfBirth,
                    Gender = s.Gender,
                    GradeCode = s.GradeCode,
                    GradeName = s.GradeName,
                    ClassCode = s.ClassCode,
                    ClassName = s.ClassName,       
                    Meals = s.Meals
                }).ToList();

            if (listStudent != null && listStudent.Any())
                {             
                // filter students list

                if(type == "submitted")
                {
                     listStudent = listStudent.Where(student => student.Meals != null && student.Meals.Count() > 1).ToList();
                } else if ( type == "active")
                {
                    listStudent = listStudent.Where(student =>
                    student.Meals != null && student.Meals.Any() && student.Meals.Count() > 0
                    ).ToList();

                    // Filter meals for each student
                    foreach (var student in listStudent)
                    {
                        student.Meals = student.Meals.Where(meal => meal.Status == 0).ToList();
                    }
                }

                var stream = new MemoryStream();

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("StudentData");
                    var headerBackgroundColor = System.Drawing.Color.FromArgb(198, 224, 180);
                    worksheet.Cells.Style.Font.Name = "Cambria";

                    var titleRow = worksheet.Cells["A1:O1"];
                    titleRow.Merge = true;
                    titleRow.Style.Font.Size = 20;
                    titleRow.Style.Font.Bold = true;
                    titleRow.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    titleRow.Value = "LIST OF STUDENTS HAVE BREAKFAST-NOT HAVE BREAKFAST";

                    var headers = new string[][]
                     {
                                new string[] { "NO.", "STUDENT CODE", "STUDENT NAME", "CLASS", "GRADE", "MEAL REGISTRATION", "MEAL PERIOD","", "", "RECEIVED DATE", "RECEIVED BY", "RECEIVED CHANNEL", "REASON", "SUBMISSION FORM FOR NO BREAKFAST/ALL MEALS", "NOTE" },
                                new string[] { "", "", "", "", "", "", "From", "To", "No. of day", "", "", "", "", "","" }
                     };

                    for (int row = 0; row < headers.Length; row++)
                    {
                        for (int col = 0; col < headers[row].Length; col++)
                        {
                            var headerCell = worksheet.Cells[row + 2, col + 1];
                            headerCell.Value = headers[row][col];
                            headerCell.Style.Font.Bold = true;
                            headerCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Center-align header text
                            headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            headerCell.Style.Fill.BackgroundColor.SetColor(headerBackgroundColor); // Set green background

                        }

                    }

                    worksheet.Cells["G2:I2"].Merge = true;
                    worksheet.Cells["A2:A3"].Merge = true;
                    worksheet.Cells["A2:A3"].Style.WrapText = true;

                    worksheet.Cells["B2:B3"].Merge = true;
                    worksheet.Cells["B2:B3"].Style.WrapText = true;

                    worksheet.Cells["C2:C3"].Merge = true;
                    worksheet.Cells["C2:C3"].Style.WrapText = true;

                    worksheet.Cells["D2:D3"].Merge = true;
                    worksheet.Cells["D2:D3"].Style.WrapText = true;

                    worksheet.Cells["E2:E3"].Merge = true;
                    worksheet.Cells["E2:E3"].Style.WrapText = true;

                    worksheet.Cells["F2:F3"].Merge = true;
                    worksheet.Cells["F2:F3"].Style.WrapText = true;

                    worksheet.Cells["J2:J3"].Merge = true;
                    worksheet.Cells["J2:J3"].Style.WrapText = true;

                    worksheet.Cells["K2:K3"].Merge = true;
                    worksheet.Cells["K2:K3"].Style.WrapText = true;

                    worksheet.Cells["L2:L3"].Merge = true;
                    worksheet.Cells["L2:L3"].Style.WrapText = true;

                    worksheet.Cells["M2:M3"].Merge = true;
                    worksheet.Cells["M2:M3"].Style.WrapText = true;

                    worksheet.Cells["N2:N3"].Merge = true;
                    worksheet.Cells["N2:N3"].Style.WrapText = true;

                    worksheet.Cells["O2:O3"].Merge = true;
                    worksheet.Cells["O2:O3"].Style.WrapText = true;


                    // Auto-fit columns for better readability
                    worksheet.Cells.AutoFitColumns();

                    // Set the column width for the merged cells
                    worksheet.Column(1).Width = 5;  // Column A
                    worksheet.Column(2).Width = 20;  // Column B
                    worksheet.Column(3).Width = 30;  // Column C
                    worksheet.Column(4).Width = 20;  // Column D
                    worksheet.Column(5).Width = 20;  // Column E
                    worksheet.Column(6).Width = 20;  // Column F
                    worksheet.Column(7).Width = 13;  // Column G
                    worksheet.Column(8).Width = 13;  // Column H
                    worksheet.Column(9).Width = 13;  // Column I
                    worksheet.Column(10).Width = 20; // Column J
                    worksheet.Column(11).Width = 20; // Column K
                    worksheet.Column(12).Width = 20; // Column L
                    worksheet.Column(13).Width = 20; // Column M
                    worksheet.Column(14).Width = 30; // Column N
                    worksheet.Column(15).Width = 20; // Column O

                    // Add data rows using a foreach loop
                    var rowIndex = 4;
                    var rowNo = 1;
                    string previousStudentCode = null;
                    string previousStudentName = null;
                    string previousClassName = null;
                    string previousGradeName = null;

                    int mergeStartRow = rowIndex;

                    
                    foreach (var student in listStudent)
                    {
                        if (student != null && student.Meals != null && student.Meals.Any())
                        {
                            foreach(var meal in student.Meals)
                            {
                                if (student?.StudentCode != previousStudentCode)
                                {
                                    // Merge cells in the "Student" column when the value changes
                                    if (student?.StudentCode != previousStudentCode ||
                                        student?.StudentName != previousStudentName ||
                                        student?.ClassName != previousClassName ||
                                        student?.GradeName != previousGradeName)
                                    {
                                        // Merge cells in the specified columns when the values change
                                        if (previousStudentCode != null)
                                        {
                                            MergeCells(worksheet, 2, mergeStartRow, rowIndex - 1);
                                            MergeCells(worksheet, 3, mergeStartRow, rowIndex - 1);
                                            MergeCells(worksheet, 4, mergeStartRow, rowIndex - 1);
                                            MergeCells(worksheet, 5, mergeStartRow, rowIndex - 1);
                                        }
                                    }
                                   
                                    previousStudentCode = student.StudentCode;
                                    previousStudentName = student.StudentName;
                                    previousClassName = student.ClassName;
                                    previousGradeName = student.GradeName;

                                    
                                    mergeStartRow = rowIndex;
                                }

                                // Set the text alignment to center horizontally and vertically
                                worksheet.Cells[rowIndex, 2, rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[rowIndex, 2, rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                                worksheet.Cells[rowIndex, 1].Value = rowNo;
                                worksheet.Cells[rowIndex, 2].Value = student.StudentCode;
                                worksheet.Cells[rowIndex, 3].Value = student.StudentName;
                                worksheet.Cells[rowIndex, 4].Value = student.ClassName;
                                worksheet.Cells[rowIndex, 5].Value = student.GradeName;
                                worksheet.Cells[rowIndex, 6].Value = meal?.MealRegistration == "AllDay" ? "No all meals" : meal?.MealRegistration == "Breakfast" ? "No breakfast" : "";
                                worksheet.Cells[rowIndex, 7].Value = meal?.FromDate.ToString("dd/MM/yyyy");
                                worksheet.Cells[rowIndex, 8].Value = meal?.ToDate.ToString("dd/MM/yyyy");
                                worksheet.Cells[rowIndex, 9].Value = meal?.TotalNumberofdays;
                                worksheet.Cells[rowIndex, 10].Value = meal?.CreatedAt.ToString("dd/MM/yyyy");
                                worksheet.Cells[rowIndex, 11].Value = null;
                                worksheet.Cells[rowIndex, 12].Value = null;
                                worksheet.Cells[rowIndex, 13].Value = meal?.Reason;
                                worksheet.Cells[rowIndex, 14].Value = student?.Meals?.Count > 1 ? "Yes" : "Not yet";
                                worksheet.Cells[rowIndex, 15].Value = null;

                                // Check meal status and set background color to red for the entire row (columns F to O)
                                if (meal?.Status == 1)
                                {
                                    for (int col = 6; col <= 15; col++)
                                    {
                                        worksheet.Cells[rowIndex, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        worksheet.Cells[rowIndex, col].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(250, 230, 195));
                                    }
                                }
                                rowIndex++;
                                rowNo++;
                                
                            }
                            // Merge the last group of cells if needed
                            if (previousStudentCode != null)
                            {
                                MergeCells(worksheet, 2, mergeStartRow, rowIndex - 1);
                                MergeCells(worksheet, 3, mergeStartRow, rowIndex - 1);
                                MergeCells(worksheet, 4, mergeStartRow, rowIndex - 1);
                                MergeCells(worksheet, 5, mergeStartRow, rowIndex - 1);
                            }
                        } else
                        {
                            worksheet.Cells[rowIndex, 1].Value = rowNo;
                            worksheet.Cells[rowIndex, 2].Value = student.StudentCode;
                            worksheet.Cells[rowIndex, 3].Value = student.StudentName;
                            worksheet.Cells[rowIndex, 4].Value = student.ClassName;
                            worksheet.Cells[rowIndex, 5].Value = student.GradeName;
                            rowIndex++;
                            rowNo++;
                        }
                        
                    }
                    

                    // Function to merge cells in a column
                    void MergeCells(ExcelWorksheet ws, int column, int startRow, int endRow)
                    {
                        var range = ws.Cells[startRow, column, endRow, column];
                        range.Merge = true;
                    }
                    

                    worksheet.Cells[worksheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[worksheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[worksheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[worksheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    _fileName = $"MealsRegistrationReport-{DateTime.Now.ToString("yyyyMMddTHHmmss")}.xlsx";
                    FileInfo file = new FileInfo(Path.Combine(_filePath, _fileName));

                    package.SaveAs(file);
                  
                }

            }
            return Json(new { filename = _fileName });
           
          
        }

        [HttpGet]
        public ActionResult Download(string fileName)
        {
            var file = Path.Combine(_filePath, fileName);
            byte[] fileByteArray = System.IO.File.ReadAllBytes(file);
            System.IO.File.Delete(file);
            return File(fileByteArray, "application/octet-stream", fileName);
                       
        }

    }
}
