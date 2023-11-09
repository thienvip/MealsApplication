
using RestSharp;

namespace MealApplication.Extensions
{
    public interface INotificationExtension
    {
        bool sendSMS(string phone_number, string message);
    }
    public class NotificationExtension : INotificationExtension
    {
        public bool sendSMS(string phone_number, string message)
        {
            Uri baseUrl = new Uri("https://cloudsms.vietguys.biz:4438/api/");
            IRestClient client = new RestClient(baseUrl);
            IRestRequest request = new RestRequest(Method.GET);
            //Parameter
            request.AddParameter("u", "xxx", ParameterType.QueryString);
            request.AddParameter("pwd", "xxx", ParameterType.QueryString);
            request.AddParameter("from", "VIET UC", ParameterType.QueryString);
            request.AddParameter("phone", phone_number, ParameterType.QueryString);
            request.AddParameter("sms", message, ParameterType.QueryString);

            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                return true;
            }
            return false;
        }
    }
}
