using src.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Web.Common.Models.ExportTypeModels
{
    public class ExportTypeModels
    {
        public string Type { get; set; }
        public List<Student> ListStudent { get; set; }
    }
}
