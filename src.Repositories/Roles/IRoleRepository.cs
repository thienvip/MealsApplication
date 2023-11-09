using src.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Repositories.Roles
{
    public interface IRoleRepository
    {
        Task<IList<Role>> GetAllRolesAsync();

        Task<IList<Role>> GetRolesForUserAsync(int userId);

        Task<IList<UserRole>> GetUserRolesForUserAsync(int userId);
    }
}
