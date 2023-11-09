using Novell.Directory.Ldap;
using src.Repositories.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.NovellDirectoryLdap
{
    public class LdapAuthenticationService: IAuthenticationService
    {
        public bool ValidateUser(string domainName, string username, string password)
        {
            string userDn = $"{username}@{domainName}";
            try
            {
                using (var connection = new LdapConnection { SecureSocketLayer = false })
                {
                    connection.Connect(domainName, LdapConnection.DefaultPort);
                    connection.Bind(userDn, password);
                    if (connection.Bound)
                        return true;
                }
            }catch (LdapException)
            {
                throw;
            }
            return false;
        }
    }
}
