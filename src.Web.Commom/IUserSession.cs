using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Web.Common
{
    public interface IUserSession
    {
        bool IsAuthenticated { get; }
        int Id { get; }
        string FirstName { get; }
        string LastName { get; }
        string UserName { get; }
        string FullName { get; }
        string EMRSystem { get; }
        string ViewType { get; }

        bool IsInRole(string roleName);
    }
}
