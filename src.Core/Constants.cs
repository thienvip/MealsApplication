using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Core
{
    public class Constants
    {
        public static class Areas
        {
            public const string Administration = "Administration";
        }

        public static class EmailTemplates
        {
            public const string AddNewUserNotification = "Add New User Notification";
        }

        /// <summary>
        /// Cache time in minutes.
        /// </summary>
        public static class CacheTimes
        {
            public static TimeSpan DefaultTimeSpan = TimeSpan.FromMinutes(60);
        }

        public static class MainPages
        {
            // System pages
            public const string AccessDenied = "Access Denied";
            public const string AntiForgery = "Oops!";
            public const string Dashboard = "Dashboard";
            public const string EmailTemplates = "Email Templates";
            public const string EmailTemplateEdit = "Edit Email Template";
            public const string Error = "Error";
            public const string Login = "Login";
            public const string ForgotPassWord = "Forgot PassWord";
            public const string Logs = "Application Logs";
            public const string PageNotFound = "Page Not Found";
            public const string ReleaseHistory = "Release History";
            public const string Settings = "Settings";
            public const string SettingEdit = "Edit Setting";
            public const string Users = "Users";
            public const string UserCreate = "Add New User";
            public const string UserEdit = "Edit User";

            public const string News = "News";
            public const string NewsEdit = "Edit News";

            public const string InvitationLetter = "Invitation Letters";
            public const string InvitationLetterEdit = "Edit";

            // Application pages
            public const string Home = "Trường Quốc tế Việt Úc (VAS)";
            public const string Sample = "Sample";
            public const string ApplicationUsers = "Application Users";
            public const string Notification = "Notification";

        }

        public static class Messages
        {
            public const string Error = @"<h4>An error occurred while processing your request.</h4><p>If these issue persists, then please contact customer service.</p>";

            public const string PageNotFound = @"<h4>Sorry, the page you're looking for cannot be found.</h4><p>If these issue persists, then please contact customer service.</p>";

            public const string AntiForgery = @"<h4>You tried to submit the same page twice.</h4><p>You appear to have submitted a page twice (often caused by pressing the back button and trying again).</p><p>To avoid getting these errors, simply refresh the page containing the form you wish to submit and try again.</p>";

            public const string AccessDenied = "<h4>You do not have access to the application.</h4><p>If you think this is an error, then please contact your manager.</p>";
        }

        public static class RoleNames
        {
            public const string Administrator = "Administrator";
            public const string User = "User";
        }
        public static class ContactEmailType
        {
            public const string HomeEmail = "Home";
            public const string FatherEmail = "Father Email";
            public const string MotherEmail = "Mother Email";
        }
        public static class StaticVariables
        {
            public const string AllCampus = "ALL";
        }
        public static class Permission
        {
            public const string EditPhoneNumber = "EditPhoneNumber";
        }
        public static class OnePayConfig
        {
            public const int vpc_Version = 2;
            public const string vpc_Currency = "VND";
            public const string vpc_Command = "pay";
            public const string vpc_Locale = "vn";
        }
    }
}

