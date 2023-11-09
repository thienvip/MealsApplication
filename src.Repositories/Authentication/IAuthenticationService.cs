using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Repositories.Authentication
{
    public interface IAuthenticationService
    {
        bool ValidateUser(string domain, string userName, string password);
    }
}
