using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Core.Domains
{
    public class grade
    {
        [Key]
        public string code { get; set; }
        public string name { get; set; }
        public string next_grade { get; set; }
    }
}
