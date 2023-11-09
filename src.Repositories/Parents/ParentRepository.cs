using Microsoft.EntityFrameworkCore;
using src.Core.Domains;
using src.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Repositories.Parents
{
    public class ParentRepository : IParentRepository
    {
        private readonly IDbContext _context;

        public ParentRepository(IDbContext context)
        {
            _context = context;
           
        }

        public Parent GetParentsByEmailAddressFromPhoebus(string email)
        {
            
             return _context.Parents.FromSql($"Parent_GetByEmailAddress @EmailAddress = {email}").AsEnumerable().FirstOrDefault();
        }

        public  Parent GetParentsByPhoneNumberFromPhoebus(string phoneNumber)
        {
            return  _context.Parents.FromSql($"parent_GetByPhoneNumber @phoneNumber = {phoneNumber}").AsEnumerable().FirstOrDefault();
        }

        public  Parent getParentByEmail(string email)
        {
            return  _context.Parents.Where(p => p.Email.ToUpper() == email.ToUpper()).AsEnumerable().FirstOrDefault();
        }

        public async Task<Parent> getParentByPhone(string phone)
        {
            return await _context.Parents.Where(p => p.PhoneNumber == phone).FirstOrDefaultAsync();
        }
        public async Task updateParent(Parent model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            _context.Parents.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task insertParent(Parent model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var existingParent = _context.Parents.Where(p => p.Id == model.Id).AsNoTracking().FirstOrDefault();
            if (existingParent == null)
            {
                _context.Parents.Add(model);
            }
            await _context.SaveChangesAsync();
        }
    }
}
