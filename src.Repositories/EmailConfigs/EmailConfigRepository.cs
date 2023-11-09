using Microsoft.EntityFrameworkCore;
using src.Core.Domains;
using src.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace src.Repositories.EmailConfigs
{
    public class EmailConfigRepository : IEmailConfigRepository
    {
        private readonly IDbContext _dbContext;
        public EmailConfigRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task addEmaiConfig(EmailConfig model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            _dbContext.EmailConfigs.Add(model);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<EmailConfig> getAllConfig()
        {
            try
            {
                return await _dbContext.EmailConfigs
               .Include(e => e.confirmation)
               .Include(e => e.validate)
               .Include(e => e.sms_validate)
               .Include(e => e.confirmation_success)
               .Include(e => e.email_not_yet_confirmed)
               .Include(e => e.sms_not_yet_confirmed)
               .Include(e => e.confirmation_meal)
               .FirstOrDefaultAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task removeAllconfigs()
        {
            _dbContext.EmailConfigs.RemoveRange(_dbContext.EmailConfigs);
            await _dbContext.SaveChangesAsync();
        }
    }
}