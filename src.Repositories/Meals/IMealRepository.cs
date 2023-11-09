using src.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Repositories.Meals
{
    public interface IMealRepository
    {
        Task<List<Meal>> GetAll();
        Task<Meal> GetById(int id);
        void Insert(Meal data);
        void Update(Meal data);
        void Delete(Meal data);
        Task<List<Meal>> GetByStudentCode(string studentCode);
    }
}
