using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SharePointPOC
{
    class SharepointCredentials
    {
        public static string SharePointUrl= "";
        public static string UserName = "";
        public static string Password = "";

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
