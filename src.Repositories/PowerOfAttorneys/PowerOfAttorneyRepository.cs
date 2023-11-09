using Microsoft.EntityFrameworkCore;
using src.Core.Domains;
using src.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Repositories.PowerOfAttorneys
{
    public class PowerOfAttorneyRepository : IPowerOfAttorneyRepository
    {
        private readonly IDbContext _dbContext;

        public PowerOfAttorneyRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PowerOfAttorney>> GetAll()
        {
            return await _dbContext.PowerOfAttorneys.AsQueryable().ToListAsync();
        }

        public async Task<PowerOfAttorney> GetById(int id)
        {
            return await _dbContext.PowerOfAttorneys.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public void Insert(PowerOfAttorney data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            _dbContext.PowerOfAttorneys.Add(data);
            _dbContext.SaveChanges();
        }

        public void Update(PowerOfAttorney data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            data.LastUpdated = DateTime.Now;
            _dbContext.PowerOfAttorneys.Update(data);
            // _dbContext.SaveChangesAsync();
            _dbContext.SaveChanges();
        }

        public async Task<List<PowerOfAttorney>> GetByStudentCode(string studentCode)
        {
            return await _dbContext.PowerOfAttorneys.Include(p => p.AuthorizedPerson)
                .Where(p => p.StudentCode.Trim().ToUpper() == studentCode.ToUpper()).ToListAsync();
        }

        public async Task<List<PowerOfAttorney>> GetByMandatorId(int mandatorId)
        {
            return await _dbContext.PowerOfAttorneys.Where(p => p.MandatorId == mandatorId).ToListAsync();
        }
    }
}

