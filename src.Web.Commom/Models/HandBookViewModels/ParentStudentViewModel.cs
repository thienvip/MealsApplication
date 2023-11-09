
using src.Core.Domains;

namespace src.Web.Common.Models.HandBookViewModels
{
    public class ParentStudentViewModel
    {
        public Parent Parent { get; set; }
        public List<Student> Students { get; set; }
        public Mandator Mandators { get; set; }

       
    }
}