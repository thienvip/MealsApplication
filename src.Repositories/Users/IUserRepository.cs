using src.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Repositories.Users
{
    public interface IUserRepository
    {
        Task<IList<User>> GetUsersAsync();
        Task<User> GetUserByUserNameAsync(string userName);
        Task<User> GetUserByIdAsync(int userId);
        Task<int> AddUserAsync(User user);
        Task UpdateUserAsync(User user);
    }
}
