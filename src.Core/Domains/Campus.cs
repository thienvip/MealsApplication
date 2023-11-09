using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Core.Domains
{
    public partial class Campus
    {
        public Campus()
        {
            UserCampus = new HashSet<UserCampus>();
        }
        [Key]
        public string code { get; set; }
        public string name { get; set; }
        public string access_code { get; set; }
        public string merchant_id { get; set; }
        public string hash_code { get; set; }
        public virtual ICollection<UserCampus> UserCampus { get; set; }
    }
}
