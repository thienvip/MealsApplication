using System.ComponentModel.DataAnnotations;
using src.Localization.Resources;

namespace src.Web.Common.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "UserName", ResourceType = typeof(SharedResource))]
        [Required(ErrorMessageResourceName = "EnterUsername", ErrorMessageResourceType = typeof(SharedResource))]
        [RegularExpression(@"^[0-9a-zA-Z\.]*$", ErrorMessageResourceName = "RegularExpression_Username", ErrorMessageResourceType = typeof(SharedResource))]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(SharedResource))]
        [Required(ErrorMessageResourceName = "EnterPassword", ErrorMessageResourceType = typeof(SharedResource))]
        public string Password { get; set; }
    }
}
