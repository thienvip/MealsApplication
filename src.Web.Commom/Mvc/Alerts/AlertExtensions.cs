using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace src.Web.Common.Mvc.Alerts
{
    public static class AlertExtensions
    {
        private const string AlertsKey = "alerts";

        public static IList<Alert> GetAlerts(this ITempDataDictionary tempData)
        {
            if (!tempData.ContainsKey(AlertsKey))
            {
                tempData.Set(AlertsKey, new List<Alert>());
            }
            return tempData.Get<List<Alert>>(AlertsKey);
        }


        public static void SetAlerts(this ITempDataDictionary tempData, IList<Alert> alerts)
        {
            tempData.Set(AlertsKey, alerts);
        }
        public static IActionResult WithError(this IActionResult result, string message)
        {
            return new AlertDecoratorResult(result, "error", message);
        }
    }
}
