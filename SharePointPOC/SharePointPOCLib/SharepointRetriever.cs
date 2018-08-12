using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointPOCLib
{
    public class SharepointRetriever
    {
        public static void Retrieve()
        {
            try
            {
                LitigationDocument.RetrieveLitigation();
                LegalDocuments.RetrieveLegalDocuments();
                InternalInvestigationDocument.RetrieveInvestigation();
                Console.WriteLine("Sharepoint retrieving Completed successfully");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Sharepoint error ");
                Console.WriteLine(ex);
                Console.Write(ex.Message);
            }
        }
    }
}
