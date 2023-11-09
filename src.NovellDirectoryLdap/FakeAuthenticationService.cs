using src.Repositories.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.NovellDirectoryLdap
{
    public class FakeAuthenticationService : IAuthenticationService
    {
        public bool ValidateUser(string domain, string userName, string password)
        {
            return domain == "domain2.com" && userName == "johndoe" && password == "Sw@*V9Rk";
        }
    }
}
