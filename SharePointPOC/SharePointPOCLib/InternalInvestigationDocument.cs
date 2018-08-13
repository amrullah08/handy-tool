using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SharePointPOCLib
{
    public class InternalInvestigationDocument
    {
        ListItem listItem;
        public static List<InternalInvestigationDocument> InternalInvestigationDocuments = new List<InternalInvestigationDocument>();

        public InternalInvestigationDocument(ListItem listItem)
        {
            this.listItem = listItem;
        }
        public string FileUrl
        {
            get
            {
                ///sites/ChrevronBot/Shared Documents/Sample Doc Legal Hold - PDF1.pdf
                return SharepointCredentials.SharePointSite + (this.listItem.FieldValues["FileRef"].ToString());
            }
        }

        public int YearOfInternalInvestigation
        {
            get
            {
                return Convert.ToInt32(listItem.FieldValues["Year_x0020_of_x0020_Internal_x00"]);
            }
        }

        public string Title
        {
            get
            {
                return (listItem.FieldValues["Title"]).ToString().Split('/').Last().ToString();
            }
        }

        public static void RetrieveInvestigation()
        {
            Console.WriteLine("Retrieving Investigation document");
            string webSPOUrl = SharepointCredentials.SharePointUrl;
            string userName = SharepointCredentials.UserName;
            InternalInvestigationDocuments.Clear();
            SecureString password = SharepointCredentials.FetchPassword();
            using (var context = new ClientContext(webSPOUrl))
            {

                context.Credentials = new SharePointOnlineCredentials(userName, password);
                Web web = context.Web;
                // Assume the web has a list named "Announcements". 
                List announcementsList = context.Web.Lists.GetByTitle("Internal Investigation Case Tracking");

                // This creates a CamlQuery that has a RowLimit of 100, and also specifies Scope="RecursiveAll" 
                // so that it grabs all list items, regardless of the folder they are in. 
                CamlQuery query = CamlQuery.CreateAllItemsQuery(100);
                ListItemCollection items = announcementsList.GetItems(query);

                // Retrieve all items in the ListItemCollection from List.GetItems(Query). 
                context.Load(items);
                context.ExecuteQuery();
                foreach (ListItem listItem in items)
                {
                    InternalInvestigationDocuments.Add(new InternalInvestigationDocument(listItem));
                }
            }
        }
    }
}
