using System;  
using System.Security;  
using Microsoft.SharePoint.Client;  
using System.Collections.Generic;  
using System.Linq;  
using System.Text;  
using System.Threading.Tasks;
using SharePointPOC;

namespace ConnectToSPO
{
    class Program
    {
        static void Main(string[] args)
        {
            SharepointRetriever.Retrieve();
        }
    }
}  