using Microsoft.Extensions.Localization;
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
    public class Meal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessageResourceName = "EnterStudentCode", ErrorMessageResourceType = typeof(SharedResource))]
        [MaxLength(100, ErrorMessageResourceName = "Exceed100Characters", ErrorMessageResourceType = typeof(SharedResource))]
        public string? StudentCode { get; set; }
        public int? ParentId { get; set; }
   
        public DateTime CreatedAt { get; set; }
        public string ClassName { get; set; }

        [Required(ErrorMessageResourceName = "EnterStudentName", ErrorMessageResourceType = typeof(SharedResource))]
        [MaxLength(100, ErrorMessageResourceName = "Exceed100Characters", ErrorMessageResourceType = typeof(SharedResource))]
        public string? StudentName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessageResourceName = "EnterFromDate", ErrorMessageResourceType = typeof(SharedResource))]
        public DateTime FromDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessageResourceName = "EnterToDate", ErrorMessageResourceType = typeof(SharedResource))]
        public DateTime ToDate { get; set; }

        [Required(ErrorMessage = "Meal Registration is required.")]
        public string? MealRegistration {  get; set; }

        [Required(ErrorMessageResourceName = "EnterReason", ErrorMessageResourceType = typeof(SharedResource))]
        [MaxLength(100, ErrorMessageResourceName = "Exceed100Characters", ErrorMessageResourceType = typeof(SharedResource))]
        public string? Reason { get; set; }

        [Required(ErrorMessageResourceName = "EnterTotalNumberOfDays", ErrorMessageResourceType = typeof(SharedResource))]
        public int? TotalNumberofdays { get; set; }

        public int? Status { get; set; }

       
    }
}
