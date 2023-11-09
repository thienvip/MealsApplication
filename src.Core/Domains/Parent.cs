
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using src.Core.Enums;

namespace src.Core.Domains
{
    public class Parent
    {
        [Key]
        public int Id { get; set; }
        public string ContactType{get;set;}
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string VerifyCode{get;set;}
        public int ResentSms{get;set;}
        public int Status{get;set;}
        public DateTime? CreatedAt{get;set;}
        public DateTime UpdatedAt {get;set;}
        [NotMapped]
        public bool IsVerified{get;set;}
        [NotMapped]
        public LoginTypeEnum LoginType{get;set;}
        [NotMapped]
        public ConfirmProcessEnum ConfirmProcess{get;set;}

       

    }
}
