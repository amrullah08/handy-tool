using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SharePointPOCLib
{
    public class LitigationDocument
    {
        ListItem listItem;
        public static List<LitigationDocument> LitigationDocuments = new List<LitigationDocument>();
        public LitigationDocument(ListItem listItem)
        {
            this.listItem = listItem;
        }
        public string Title
        {
            get
            {
                return (listItem.FieldValues["FileRef"]).ToString().Split('/').Last().ToString();
            }
        }

        public DateTime DateTracking
        {
            get
            {
                return Convert.ToDateTime(listItem.FieldValues["DateTracking"]);
            }
        }

        public bool ActiveCase
        {
            get
            {
                return Convert.ToBoolean(listItem.FieldValues["ActiveCase"].Equals("Yes"));
            }
        }


        public static void RetrieveLitigation()
        {
            Console.WriteLine("Retrieving Litigation document");
            string webSPOUrl = SharepointCredentials.SharePointUrl;
            string userName = SharepointCredentials.UserName;
            SecureString password = SharepointCredentials.FetchPassword();
            LitigationDocuments.Clear();
            using (var context = new ClientContext(webSPOUrl))
            {

                context.Credentials = new SharePointOnlineCredentials(userName, password);
                Web web = context.Web;
                // Assume the web has a list named "Announcements". 
                List announcementsList = context.Web.Lists.GetByTitle("LitigationCases");

                // This creates a CamlQuery that has a RowLimit of 100, and also specifies Scope="RecursiveAll" 
                // so that it grabs all list items, regardless of the folder they are in. 
                CamlQuery query = CamlQuery.CreateAllItemsQuery(100);
                ListItemCollection items = announcementsList.GetItems(query);

                // Retrieve all items in the ListItemCollection from List.GetItems(Query). 
                context.Load(items);
                context.ExecuteQuery();
                foreach (ListItem listItem in items)
                {
                    LitigationDocuments.Add(new LitigationDocument(listItem));
                }
            }
        }
    }
}
