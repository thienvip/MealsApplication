using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Core
{
    public class AppSettings
    {
        public Application Application { get; set; }
        public CookieAuthentication CookieAuthentication { get; set; }
        public CCEmailSettings CCEmailSettings { get; set; }
    }

    public class Application
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Domain { get; set; }
        public string HandBookGoogleFileId { get; set; }
        public string SchoolRulesGoogleFileId { get; set; }
    }

    public class CCEmailSettings
    {
        public string BTH { get; set; }
        public string PXL { get; set; }
        public string SR { get; set; }
        public string RS { get; set; }
        public string GH { get; set; }
        public string SL { get; set; }
        public string HVT { get; set; }
    }

    public class CookieAuthentication
    {
        /// <summary>
        /// Session expiration in minutes. Default is 15 minutes
        /// </summary>
        public int ExpireMinutes { get; set; } = 15;

        /// <summary>
        /// Display a message box before session expires. Default is 2 minutes.
        /// </summary>
        public int SessionExpireNotificationMinutes { get; set; } = 2;
    }
}
