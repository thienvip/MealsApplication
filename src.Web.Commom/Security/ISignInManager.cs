using src.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Web.Common.Security
{
    public interface ISignInManager
    {
        Task SignInAsync(User user, IList<string> roleNames);
        Task SignOutAsync();
    }
}
