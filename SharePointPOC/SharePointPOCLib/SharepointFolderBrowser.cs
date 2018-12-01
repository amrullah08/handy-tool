using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SharePointPOCLib
{
    public class SharepointFolderBrowser
    {
        public static void Browser()
        {
            Console.WriteLine("Retrieving Litigation document");
            string webSPOUrl = SharepointCredentials.SharePointUrl;
            string userName = SharepointCredentials.UserName;
            SecureString password = SharepointCredentials.FetchPassword();

            using (ClientContext ctx = new ClientContext(webSPOUrl))
            {
                ctx.Credentials = new SharePointOnlineCredentials(userName, password);
                var web = ctx.Web;

                var list = ctx.Web.Lists.GetByTitle("Outlook Flow");
                ctx.Load(ctx.Web.Lists);
                ctx.ExecuteQuery();

                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml =
                           @"<View>
                       
                              </View>";
                ListItemCollection listItems = list.GetItems(camlQuery);
                ctx.Load(listItems);
                ctx.ExecuteQuery();


                foreach (ListItem listItem in listItems)
                {
                    if (listItem.FileSystemObjectType == FileSystemObjectType.File)
                    {

                    }

                    //listitem["ID"]
                    //listitem["Name"]
                }
            }

            using (var context = new ClientContext(webSPOUrl))
            {

                context.Credentials = new SharePointOnlineCredentials(userName, password);

                List sharedDocumentsList = context.Web.Lists.GetByTitle("Shared Documents");
                FolderCollection collFolder = context.Web.Folders;
                context.Load(collFolder);
                context.ExecuteQuery();

                Console.WriteLine("The current site contains the following folders:\n\n");
                foreach (Folder myFolder in collFolder)
                    Console.WriteLine(myFolder.Name);

                Web web = context.Web;
                var k = web.Folders;
                // Assume the web has a list named "Announcements". 
                List announcementsList = context.Web.Lists.GetByTitle("Outlook Flow");

                // This creates a CamlQuery that has a RowLimit of 100, and also specifies Scope="RecursiveAll" 
                // so that it grabs all list items, regardless of the folder they are in. 
                CamlQuery query = CamlQuery.CreateAllItemsQuery(100);
                ListItemCollection items = announcementsList.GetItems(query);

                // Retrieve all items in the ListItemCollection from List.GetItems(Query). 
                context.Load(items);
                context.ExecuteQuery();
                foreach (ListItem listItem in items)
                {
                    if (listItem.FileSystemObjectType == FileSystemObjectType.File)
                    {

                    }

                        Console.WriteLine("first");//)
                    //LitigationDocuments.Add(new LitigationDocument(listItem));
                }
            }
        }
    }
}
