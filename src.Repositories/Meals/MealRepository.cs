using Microsoft.EntityFrameworkCore;
using src.Core.Domains;
using src.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Repositories.Meals
{
    public class MealRepository : IMealRepository
    {
        private readonly IDbContext _dbContext;

        public MealRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Meal>> GetAll()
        {
            return await _dbContext.Meals.AsQueryable().ToListAsync();
        }

        public async Task<Meal> GetById(int id)
        {
            return await _dbContext.Meals.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public void Insert(Meal meal)
        {
            if (meal == null) throw new ArgumentNullException(nameof(meal));
            _dbContext.Meals.Add(meal);
            _dbContext.SaveChanges();
        }

        public void Update(Meal data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));     
            _dbContext.Meals.Update(data);
            _dbContext.SaveChanges();
        }

        public void Delete(Meal data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            _dbContext.Meals.Remove(data);
            _dbContext.SaveChanges();
        }

        public async Task<List<Meal>> GetByStudentCode(string studenCode)
        {
            return await _dbContext.Meals.Where(p => p.StudentCode.Trim().ToUpper() == studenCode.ToUpper()).ToListAsync();
        }
    }
}
