using src.Localization.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Core.Domains
{
    public class Student
    {
        [Key]
        [Required(ErrorMessageResourceName = "EnterStudentCode", ErrorMessageResourceType = typeof(SharedResource))]
        [MaxLength(100, ErrorMessageResourceName = "Exceed100Characters", ErrorMessageResourceType = typeof(SharedResource))]
        public string? StudentCode { get; set; }

        [Required(ErrorMessageResourceName = "EnterStudentName", ErrorMessageResourceType = typeof(SharedResource))]
        [MaxLength(100, ErrorMessageResourceName = "Exceed100Characters", ErrorMessageResourceType = typeof(SharedResource))]
        public string? StudentName { get; set; }
        public string? Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? CampusCode { get; set; }
        public string? CampusName { get; set; }
        public string? GradeCode { get; set; }

        [Required(ErrorMessage = "Grade Name is required.")]
        public string? GradeName { get; set; }
        public int ClassCode { get; set; }

        [Required(ErrorMessageResourceName = "EnterClassName", ErrorMessageResourceType = typeof(SharedResource))]
        [MaxLength(100, ErrorMessageResourceName = "Exceed100Characters", ErrorMessageResourceType = typeof(SharedResource))]
        public string? ClassName { get; set; }
        public int Note { get; set; }
        [NotMapped]
        public PowerOfAttorney? PowerOfAttorneys { get; set; }

        public virtual List<Meal>? Meals { get; set; }
        [NotMapped]
        public virtual Meal? Meal { get; set; }

    }
}
