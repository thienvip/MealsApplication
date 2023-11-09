using System;
using System.ComponentModel.DataAnnotations;

namespace src.Web.Common.Models.ParentViewModels
{
    public class ParentCheckInforViewModel
    {
        [Required(ErrorMessage = "Quý phụ huynh vui lòng nhập số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^[0-9]{1,12}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public int phone_number { get; set; }
    }
}