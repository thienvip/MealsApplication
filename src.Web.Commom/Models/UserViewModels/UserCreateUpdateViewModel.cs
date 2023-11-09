using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Web.Common.Models.UserViewModels
{
    public class UserCreateUpdateViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(50, ErrorMessage = "{0} must not exceed {1} characters.")]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(50, ErrorMessage = "{0} must not exceed {1} characters.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(50, ErrorMessage = "{0} must not exceed {1} characters.")]
        public string LastName { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; }
        public List<SelectListItem> SelectedStatus { get; set; }

        [Display(Name = "Last Login Date")]
        [DisplayFormat(DataFormatString = "{0:M/d/yy}")]
        public DateTime LastLoginDate { get; set; }

        [Display(Name = "Created Date")]
        [DisplayFormat(DataFormatString = "{0:M/d/yy}")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Last Updated Date")]
        [DisplayFormat(DataFormatString = "{0:M/d/yy}")]
        public DateTime ModifiedOn { get; set; }

        [Display(Name = "Last Updated By")]
        public string ModifiedBy { get; set; }


        //For Roles
        [Display(Name = "Roles")]
        [Required, MinLength(1, ErrorMessage = "At least one item required in work order")]
        public IList<int> SelectedRoleIds { get; set; }
        public IList<SelectListItem> AvailableRoles { get; set; }


        public UserCreateUpdateViewModel()
        {
            SelectedRoleIds = new List<int>();
            AvailableRoles = new List<SelectListItem>();
            SelectedStatus = new List<SelectListItem>
            {
                new SelectListItem{ Selected=true,Text="Active",Value="true",},
                new SelectListItem{ Selected = false,Text="InActive",Value="false"},
            };
        }
    }
}
