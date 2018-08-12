using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SharePointPOC
{
    public class LegalDocuments
    {
        ListItem listItem;
        public static List<LegalDocuments> Legaldocuments = new List<LegalDocuments>();

        public LegalDocuments(ListItem listItem)
        {
            this.listItem = listItem;
        }
        public string Title
        {
            get
            {
                return (this.listItem.FieldValues["FileRef"]).ToString().Split('/').Last().ToString();
            }
        }
        public bool OnPreservationHold
        {
            get
            {
                return Convert.ToBoolean(listItem.FieldValues["OnPreservationHold"].Equals("Yes"));
            }
        }


        public static void RetrieveLegalDocuments()
        {
            Console.WriteLine("Retrieving Legal document");
            string webSPOUrl = SharepointCredentials.SharePointUrl;
            string userName = SharepointCredentials.UserName;
            Legaldocuments.Clear();
            SecureString password = SharepointCredentials.FetchPassword();
            using (var context = new ClientContext(webSPOUrl))
            {

                context.Credentials = new SharePointOnlineCredentials(userName, password);
                Web web = context.Web;
                // Assume the web has a list named "Announcements". 
                List announcementsList = context.Web.Lists.GetByTitle("Legal Documents");

                // This creates a CamlQuery that has a RowLimit of 100, and also specifies Scope="RecursiveAll" 
                // so that it grabs all list items, regardless of the folder they are in. 
                CamlQuery query = CamlQuery.CreateAllItemsQuery(100);
                ListItemCollection items = announcementsList.GetItems(query);

                // Retrieve all items in the ListItemCollection from List.GetItems(Query). 
                context.Load(items);
                context.ExecuteQuery();
                foreach (ListItem listItem in items)
                {
                    Legaldocuments.Add(new LegalDocuments(listItem));
                }
            }
        }
    }
}
