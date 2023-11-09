using Microsoft.EntityFrameworkCore;
using src.Core.Domains;
using src.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Repositories.Category
{
    public class BaseCategoryRepository :IBaseCategoryRepository
    {
     
        private readonly IDbContext _context;

        public BaseCategoryRepository(IDbContext dbContext)
        {
            _context = dbContext;
        }


        public async Task<IList<Campus>> getCampus()
        {
            var query = _context.Campus.AsQueryable();
            return await query.ToListAsync();
        }

        public async Task<Campus> getCampusByCode(string code)
        {
            var entity = _context.Campus.Where(c => c.code == code);
            return await entity.FirstOrDefaultAsync();
        }

        public async Task<IList<grade>> getGrade()
        {
            var query = _context.grade.AsQueryable();
            return await query.ToListAsync();
        }

        public async Task<grade> getGradeByCode(string code)
        {
            var entity = _context.grade.Where(c => c.code == code);
            return await entity.FirstOrDefaultAsync();
        }

        public async Task<IList<Campus>> getCampusByUser(int UserId)
        {
            var entity = _context.Campus.Include(c => c.UserCampus).Where(uc => uc.UserCampus.Any(u => u.UserId == UserId));
            return await entity.ToListAsync();
        }


    }
}
