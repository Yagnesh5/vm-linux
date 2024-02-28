using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace OnlineRecruitment_Main.Models
{
    public class EmailViewModel
    {
        public static string GetAppsetting(string key)
        {
            return WebConfigurationManager.AppSettings[key] != null ? WebConfigurationManager.AppSettings[key].ToString() : String.Empty;
        }

        public string Smtpemail
        {
            get
            {
                return GetAppsetting("EmailId");
            }
        }
        private string Password;
        public string Smtppassword
        {
            get
            {
                return GetAppsetting("Password");
            }
        }
        private string StmpString;
        public string Stmp
        {
            get
            {
                return GetAppsetting("Smtp");
            }
        }
        public int Port
        {
            get
            {
                int a = 0;
                int.TryParse(GetAppsetting("Port"), out a);
                return a;
            }
        }
    }
}