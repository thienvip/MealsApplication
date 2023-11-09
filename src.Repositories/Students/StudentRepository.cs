using Microsoft.EntityFrameworkCore;
using src.Core.Data;
using src.Core.Domains;
using src.Data;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace src.Repositories.Students
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IDbContext _context;
        public StudentRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetAllStudentsByCampusCode(string campusCode)
        {
            var query = await _context.Students.Where(x => x.CampusCode == campusCode).ToListAsync();

            var result = query.Select(x => new Student()
            {
                StudentCode = x.StudentCode,
                StudentName = x.StudentName,
                CampusCode = x.CampusCode,
                CampusName = x.CampusName,
                DateOfBirth = x.DateOfBirth,
                Gender = x.Gender,
                GradeCode = x.GradeCode,
                GradeName = x.GradeName,
                ClassCode = x.ClassCode,
                ClassName = x.ClassName,
                Meals = x.Meals
            });
            return result.ToList();

        }


        
        public  List<Student> GetPagedAllStudentsByCampusCode(MealPagedDataRequest request, string? searchBy, int take, int skip, string sortBy, bool sortDir, out int filteredResultsCount, out int totalResultsCount)
        {



            var query =  _context.Students.Where(x => request.campusCode.Contains(x.CampusCode));
            


            // Apply search
            if (!string.IsNullOrEmpty(request.search.value))
            {
                var searchValue = request.search.value.ToLower();

                query = query.Where(s =>
                    s.StudentName.ToLower().Contains(searchValue) ||
                    s.ClassName.ToLower().Contains(searchValue) ||
                    s.GradeName.ToLower().Contains(searchValue) ||
                    s.CampusName.ToLower().Contains(searchValue)
                );
            }


            // Apply sorting
            var order = request.order[0];
            var column = request.columns[order.column];
            var columnName = column.data;
            bool ascending = order.dir == "asc";



            switch (columnName)
            {
                case "studentCode":
                    query = ascending ? query.OrderBy(s => s.StudentCode) : query.OrderByDescending(s => s.StudentCode);
                    break;
                case "studentName":
                    query = ascending ? query.OrderBy(s => s.StudentName) : query.OrderByDescending(s => s.StudentName);
                    break;
                case "className":
                    query = ascending ? query.OrderBy(s => s.ClassName) : query.OrderByDescending(s => s.ClassName);
                    break;
                case "gradeName":
                    query = ascending ? query.OrderBy(s => s.GradeName) : query.OrderByDescending(s => s.GradeName);
                    break;
                case "campusName":
                    query = ascending ? query.OrderBy(s => s.CampusName) : query.OrderByDescending(s => s.CampusName);
                    break;
                default:
                    break;
            }



            var sortedQuery = query.Select(s => new
            {
                Student = s,
                SortKey = s.StudentCode
            });


            sortedQuery = ascending ? sortedQuery.OrderBy(x => x.SortKey) : sortedQuery.OrderByDescending(x => x.SortKey);


            var result =  sortedQuery.Select(x => new Student()
            {
                StudentCode = x.Student.StudentCode,
                StudentName = x.Student.StudentName,
                CampusCode = x.Student.CampusCode,
                CampusName = x.Student.CampusName,
                DateOfBirth = x.Student.DateOfBirth,
                Gender = x.Student.Gender,
                GradeCode = x.Student.GradeCode,
                GradeName = x.Student.GradeName,
                ClassCode = x.Student.ClassCode,
                ClassName = x.Student.ClassName,
                Meals = x.Student.Meals
            }).Skip(skip).Take(take).ToList();



            totalResultsCount = _context.Students.Where(x => request.campusCode.Contains(x.CampusCode)).Count();


            if (!string.IsNullOrEmpty(request.search.value))
            {
                var searchValue = request.search.value.ToLower();
                filteredResultsCount = _context.Students.Where(x => request.campusCode.Contains(x.CampusCode)).Where(s =>
                    s.StudentName.ToLower().Contains(searchValue) ||
                    s.ClassName.ToLower().Contains(searchValue) ||
                    s.GradeName.ToLower().Contains(searchValue) ||
                    s.CampusName.ToLower().Contains(searchValue)
                ).Count();
            }
            else
            {
                filteredResultsCount = _context.Students.Where(x => request.campusCode.Contains(x.CampusCode)).Count();
            }

            return result;
        }



        public async Task<List<Student>> GetStudentByParentEmail(string email)
        {
            var students = await _context.Students.FromSql($"Student_GetByEmailAddress @EmailAddress={email}").ToListAsync();

            var studentsQuery = from s in students
                                join po in _context.PowerOfAttorneys.Include(p => p.Mandator).Include(p => p.AuthorizedPerson) on s.StudentCode equals po.StudentCode into powerOfAttorneyJoin
                                from powerOfAttorneyResult in powerOfAttorneyJoin.DefaultIfEmpty()
                                select new
                                {
                                    Student = s,
                                    PowerOfAttorneys = powerOfAttorneyResult
                                };

            var mealsQuery = from s in students
                             join meal in _context.Meals on s.StudentCode equals meal.StudentCode into mealGroup
                             from mealResult in mealGroup.Take(1).DefaultIfEmpty()
                             select new
                             {
                                 StudentCode = s.StudentCode,
                                 Meals = mealGroup.ToList(),
                                 Meal = mealResult
                             };

            var studentsWithMeals = from s in studentsQuery
                                    join m in mealsQuery on s.Student.StudentCode equals m.StudentCode
                                    select new Student()
                                    {
                                        StudentCode = s.Student.StudentCode,
                                        StudentName = s.Student.StudentName,
                                        CampusCode = s.Student.CampusCode,
                                        CampusName = s.Student.CampusName,
                                        DateOfBirth = s.Student.DateOfBirth,
                                        Gender = s.Student.Gender,
                                        GradeCode = s.Student.GradeCode,
                                        GradeName = s.Student.GradeName,
                                        ClassCode = s.Student.ClassCode,
                                        ClassName = s.Student.ClassName,
                                        PowerOfAttorneys = s.PowerOfAttorneys,
                                        Meals = m.Meals,
                                        Meal = m.Meal
                                    };

            var finalResult = studentsWithMeals.ToList();
            return finalResult;
        }

        public async Task<List<Student>> GetByStudentCodeIncludeMeal(string studentCode)
        {
            var student = await _context.Students
                           .Where(x => x.StudentCode == studentCode)
                           .ToListAsync();

            var result = (from s in student
                          join m in _context.Meals on s.StudentCode equals m.StudentCode into mealJoin
                          from mealResult in mealJoin.DefaultIfEmpty()
                          group mealResult by s into mealGroup
                          select new
                          {
                              Student = mealGroup.Key,
                              Meals = mealGroup.ToList()
                          })
              .AsEnumerable()
              .Select(x => new Student()
              {
                  StudentCode = x.Student.StudentCode,
                  StudentName = x.Student.StudentName,
                  CampusCode = x.Student.CampusCode,
                  CampusName = x.Student.CampusName,
                  DateOfBirth = x.Student.DateOfBirth,
                  Gender = x.Student.Gender,
                  GradeCode = x.Student.GradeCode,
                  GradeName = x.Student.GradeName,
                  ClassCode = x.Student.ClassCode,
                  ClassName = x.Student.ClassName,
                  Meals = x.Meals
              }).ToList();

            return result;
        }


        public async Task<List<Student>> GetAllByCampusCode(string campusCode)
        {
            var students = await _context.Students.FromSql($"student_getAll @CampusCode={campusCode}").ToListAsync();
            var result = (from s in students
                          join po in _context.PowerOfAttorneys.Include(p => p.Mandator).Include(p => p.AuthorizedPerson) on s.StudentCode equals po.StudentCode into powerOfAttorneyJoin
                          from powerOfAttorneyResult in powerOfAttorneyJoin.OrderByDescending(p => p.CreatedAt).Take(1).DefaultIfEmpty()
                          select new Student()
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
                              Meals = s.Meals,
                              PowerOfAttorneys = powerOfAttorneyResult
                          });
            return result.ToList();
        }

        public async Task<List<Student>> GetByStudentCodeIncludePowerOfAttorney(string studentCode)
        {
            var result = (from s in _context.Students
                          join po in _context.PowerOfAttorneys.Include(p => p.Mandator).Include(p => p.AuthorizedPerson) on s.StudentCode equals po.StudentCode into powerOfAttorneyJoin
                          from powerOfAttorneyResult in powerOfAttorneyJoin.Where(s => s.StudentCode == studentCode)
                          select new Student()
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
                              PowerOfAttorneys = powerOfAttorneyResult
                          });


            return await result.ToListAsync();
        }

        public async Task<List<Student>> GetStudentIncludeLatestPowerOfAttorneyByParentEmail(string email)
        {
            var students = await _context.Students.FromSql($"Student_GetByEmailAddress @EmailAddress={email}").ToListAsync();
            var result = (from s in students
                          join po in _context.PowerOfAttorneys.Include(p => p.Mandator).Include(p => p.AuthorizedPerson) on s.StudentCode equals po.StudentCode into powerOfAttorneyJoin
                          from powerOfAttorneyResult in powerOfAttorneyJoin.OrderByDescending(p => p.CreatedAt).Take(1).DefaultIfEmpty()
                          join meal in _context.Meals on s.StudentCode equals meal.StudentCode into mealJoin
                          from mealResult in mealJoin.OrderByDescending(p => p.CreatedAt).Take(1).DefaultIfEmpty()

                          select new Student()
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
                              PowerOfAttorneys = powerOfAttorneyResult,
                              Meal = mealResult
                          });
            return result.ToList();
        }


        

        public async Task InsertStudent(Student model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var existingStudent = _context.Students.Where(s => s.StudentCode == model.Meal.StudentCode).FirstOrDefault();
           
            if (existingStudent != null)
            {
                _context.Meals.Add(model.Meal);
            }
            else
            {
                throw new Exception("Student not found");
            }
            await _context.SaveChangesAsync();
        }

       
    }
}


