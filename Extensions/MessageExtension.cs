using src.Core.Domains;
using System.Net;

namespace MealApplication.Extensions
{
    public static class MessageExtension
    {
        public static string ReplaceForValidation(Parent model, string template)
        {

            var tokens = new Dictionary<string, string>()
                {
                    {"[active_code]", WebUtility.HtmlEncode(model.VerifyCode)},
            };
            foreach (string token in tokens.Keys)
                template = template.Replace(token, tokens[token]);

            return template;
        }
        public static string ReplaceForSmsValidation(Parent model, string template)
        {

            var tokens = new Dictionary<string, string>()
            {
                {"[active_code]", WebUtility.HtmlEncode(model.VerifyCode)},
            };
            foreach (string token in tokens.Keys)
                template = template.Replace(token, tokens[token]);

            return template;
        }

        public static string ReplaceForParentConfirmation(Student data, string template, string host)
        {
            var tokens = new Dictionary<string, string>()
            {
                { "[Students_FullName]", WebUtility.HtmlEncode(data.StudentName) },
                { "[Students_CampusName]", WebUtility.HtmlEncode(data.CampusName.Split('.').Last()) },
                { "[Students_ClassName]", WebUtility.HtmlEncode(data.ClassName) },
                { "[Students_GradeName]", WebUtility.HtmlEncode(data.GradeName) },

                { "[Mandators_Fullname]", WebUtility.HtmlEncode(data.PowerOfAttorneys.Mandator.FullName) },
                { "[Mandators_PhoneNumber]", WebUtility.HtmlEncode(data.PowerOfAttorneys.Mandator.PhoneNumber) },
                { "[Mandators_EmailAddress]", WebUtility.HtmlEncode(data.PowerOfAttorneys.Mandator.EmailAddress) },
                { "[Mandators_IdNumber]", WebUtility.HtmlEncode(data.PowerOfAttorneys.Mandator.IdNumber) },
                { "[Mandators_IdNumberIssuedBy]", WebUtility.HtmlEncode(data.PowerOfAttorneys.Mandator.IdNumberIssuedBy) },
                { "[Mandators_IdNumberIssuedAt]", WebUtility.HtmlEncode(data.PowerOfAttorneys.Mandator.IdNumberIssuedAt !=null ? data.PowerOfAttorneys.Mandator.IdNumberIssuedAt.Value.ToString("dd/MM/yyyy"):"") },

                { "[AuthorizedPerson_FullName]", WebUtility.HtmlEncode(data.PowerOfAttorneys.AuthorizedPerson.FullName) },
                { "[AuthorizedPerson_PhoneNumber]", WebUtility.HtmlEncode(data.PowerOfAttorneys.AuthorizedPerson.PhoneNumber) },
                { "[AuthorizedPerson_EmailAddress]", WebUtility.HtmlEncode(data.PowerOfAttorneys.AuthorizedPerson.EmailAddress) },
                { "[AuthorizedPerson_IdNumber]", WebUtility.HtmlEncode(data.PowerOfAttorneys.AuthorizedPerson.IdNumber) },
                { "[AuthorizedPerson_IdNumberIssuedBy]", WebUtility.HtmlEncode(data.PowerOfAttorneys.AuthorizedPerson.IdNumberIssuedBy) },
                { "[AuthorizedPerson_IdNumberIssuedAt]", WebUtility.HtmlEncode(data.PowerOfAttorneys.AuthorizedPerson.IdNumberIssuedAt != null ? data.PowerOfAttorneys.AuthorizedPerson.IdNumberIssuedAt.Value.ToString("dd/MM/yyyy"):"") },
                { "[AuthorizedPerson_Address]", WebUtility.HtmlEncode(data.PowerOfAttorneys.AuthorizedPerson.Address) },
                { "[AuthorizedPerson_Relationship]", WebUtility.HtmlEncode(data.PowerOfAttorneys.AuthorizedPerson.Relationship) },

                { "[Meals_CreatedAt]", WebUtility.HtmlEncode(data.Meal.CreatedAt.ToString("dd/MM/yyyy HH:mm")) },
                { "[Meals_FromDate]", WebUtility.HtmlEncode(data.Meal.FromDate.ToString("dd/MM/yyyy")) },
                { "[Meals_ToDate]", WebUtility.HtmlEncode(data.Meal.ToDate.ToString("dd/MM/yyyy")) },
                
                { "[DateOfBirth]", WebUtility.HtmlEncode(data.DateOfBirth.ToString("dd/MM/yyyy")) },

            };
            foreach (string token in tokens.Keys)
                template = template.Replace(token, tokens[token]);

            return template;
        }
    }
}
