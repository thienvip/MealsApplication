using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Web.Common.Mvc
{
    public static class SessionDataExtension
    {
        public static TValue Get<TValue>(this ISession session, string key)
        {
            if (session.TryGetValue(key, out byte[] value))
            {
                string json = System.Text.Encoding.UTF8.GetString(value);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<TValue>(json);
            }
            return default(TValue);
        }

        public static void Set<TValue>(this ISession session, string key, TValue value)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            session.Set(key, System.Text.Encoding.UTF8.GetBytes(json));
        }

        public static void Remove(this ISession session, string key)
        {
            session.Remove(key);
        }

        public static void SetExpiration(this ISession session, TimeSpan expiration)
        {
            session.SetString("SessionTimeout", DateTime.UtcNow.Add(expiration).ToString("o"));
        }

        public static bool IsSessionExpired(this ISession session)
        {
            var sessionTimeout = session.GetString("SessionTimeout");
            if (DateTime.TryParse(sessionTimeout, out DateTime timeout))
            {
                DateTime currentPlus7Hours = DateTime.UtcNow.AddHours(7);
                return timeout < currentPlus7Hours;
            }
            return true;
        }
    }
}
