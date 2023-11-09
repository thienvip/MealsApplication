using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Core
{
    public class DateTimeAdapter : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
