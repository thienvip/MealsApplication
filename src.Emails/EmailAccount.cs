using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Emails
{
    public class EmailAccount
    {
        public string LocalDomain { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
