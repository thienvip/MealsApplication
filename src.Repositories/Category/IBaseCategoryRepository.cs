using src.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Repositories.Category
{
    public interface IBaseCategoryRepository
    {
        Task<IList<Campus>> getCampus();
        Task<IList<Campus>> getCampusByUser(int UserId);
        Task<Campus> getCampusByCode(string code);
        Task<IList<grade>> getGrade();
        Task<grade> getGradeByCode(string code);
    }
}
