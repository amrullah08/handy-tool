using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace SharePointPOCLib
{
    public class SharepointCredentials
    {
        public static string SharePointSite;
        public static string SharePointDirectory;

        public static string SharePointUrl;
        static SharepointCredentials()
        {
            try
            {
                SharePointSite = ConfigurationSettings.AppSettings.Get("SharePointSite");
                SharePointDirectory = ConfigurationSettings.AppSettings.Get("SharePointDirectory");
                SharePointUrl = ((SharePointSite + "/" + SharePointDirectory));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public static string UserName = ConfigurationSettings.AppSettings.Get("UserName");
        public static string Password = ConfigurationSettings.AppSettings.Get("Password");

        public static SecureString FetchPassword()
        {
            var securePassword = new SecureString();
            //Convert string to secure string  
            foreach (char c in Password)
                securePassword.AppendChar(c);
            securePassword.MakeReadOnly();
            return securePassword;
        }
    }
}
