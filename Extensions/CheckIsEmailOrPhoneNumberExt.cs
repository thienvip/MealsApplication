using System.Text.RegularExpressions;

namespace MealApplication.Extensions
{
    public class CheckIsEmailOrPhoneNumberExt
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsPhoneNumber(string number)
        {
            return Regex.Match(number, @"^+\d{0,12}$").Success;
        }
    }
}
