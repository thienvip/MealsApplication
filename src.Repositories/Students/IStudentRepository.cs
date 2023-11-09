using src.Core.Data;
using src.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Repositories.Students
{
    public interface IStudentRepository
    { 
        Task<List<Student>> GetStudentByParentEmail(string email);
        Task InsertStudent(Student model);
        Task<List<Student>> GetAllByCampusCode(string campusCode);
        Task<List<Student>> GetByStudentCodeIncludePowerOfAttorney(string studentCode);
        Task<List<Student>> GetStudentIncludeLatestPowerOfAttorneyByParentEmail(string email);
        Task<List<Student>> GetByStudentCodeIncludeMeal(string studenCode);

        Task<List<Student>> GetAllStudentsByCampusCode(string campusCode);
        List<Student> GetPagedAllStudentsByCampusCode(MealPagedDataRequest request, string searchBy, int take, int skip, string sortBy, bool sortDir, out int filteredResultsCount, out int totalResultsCount);
    }
}
