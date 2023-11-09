using src.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Core.Data
{
    public class MealPagedDataRequest : PagedDataRequest
    {
      
       
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public string[] ? campusCode { get; set; }
        public Student students { get; set; }
        public Search search { get; set; } 

        public MealSortField SortField { get; set; }
        public SortOrder SortOrder { get; set; }    

        public List<Column> columns { get; set; } // Columns
        public List<Order> order { get; set; } // Sorting order

    }
    public class Search
    {
        public string value { get; set; }
        public string regex { get; set; }
    }

    public enum MealSortField
    {
        studentCode,
        studentName,
        className,
        gradeName,
        campusName
    }

    public class Column
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public Search search { get; set; }
    }

    public class Order
    {
        public int column { get; set; }
        public string dir { get; set; }
    }
}
