using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Description;

namespace ConsoleApp10
{
    internal class CrmSolutionHelper : ICrmSolutionHelper
    {
        public string RepositoryUrl { get; set; }
        public string RepositoryLocalFolder { get; set; }
        public string Branch { get; set; }
        public string OrganizationServiceUri { get; set; }
        private ClientCredentials clientCredentials { get; set; }
        public bool CanPush { get; set; }
        public List<SolutionFileInfo> SolutionFileInfos { get; set; }

        public CrmSolutionHelper(string repositoryUrl, string repositoryLocalFolder, string branch, string organizationServiceUrl, string userName, string password)
        {
            this.RepositoryUrl = repositoryUrl;
            this.RepositoryLocalFolder = repositoryLocalFolder;
            this.Branch = branch;
            this.OrganizationServiceUri = organizationServiceUrl;
            this.clientCredentials = new ClientCredentials();
            this.clientCredentials.UserName.UserName = userName;
            this.clientCredentials.UserName.Password = password;
        }

        public List<SolutionFileInfo> DownloadSolutionFile(string solutionUnqiueName, string owner, string message)
        {
            CanPush = false;
            Uri serviceUri = new Uri(OrganizationServiceUri);
            List<SolutionFileInfo> solutionFileInfos = new List<SolutionFileInfo>();
            Console.WriteLine("Connecting to the " + OrganizationServiceUri);
            using (OrganizationServiceProxy serviceProxy = new OrganizationServiceProxy(serviceUri, null, this.clientCredentials, null))
            {
                if (!string.IsNullOrEmpty(solutionUnqiueName))
                {
                    solutionFileInfos.Add(ExportSolution(serviceProxy, solutionUnqiueName, owner, message));
                    return solutionFileInfos;
                }

                QueryExpression querySampleSolution = new QueryExpression
                {
                    EntityName = "syed_sourcecontrolqueue",
                    ColumnSet = new ColumnSet("syed_solutionname", "syed_status", "syed_repositoryurl", "syed_comment", "syed_branch", "ownerid"),
                    Criteria = new FilterExpression()
                };

                querySampleSolution.Criteria.AddCondition("syed_status", ConditionOperator.Equal, "Queued");

                Console.WriteLine("Fetching Solutions to be copied to Repository ");
                EntityCollection querySampleSolutionResults = serviceProxy.RetrieveMultiple(querySampleSolution);

                for (int i = 0; i < querySampleSolutionResults.Entities.Count; i++)
                {
                    var solution = querySampleSolutionResults.Entities[i];
                    //Creates the Export Request
                    try
                    {
                        ///Todo: uncomment below
                        solution["syed_status"] = "Exporting Solution";
                        solution["syed_repositoryurl"] = RepositoryUrl;
                        solution["syed_branch"] = Branch;
                        serviceProxy.Update(solution);

                        ///Todo: uncomment below
                        solutionFileInfos.Add(ExportSolution(serviceProxy, solution.GetAttributeValue<string>("syed_solutionname"), solution.GetAttributeValue<EntityReference>("ownerid").Name, solution.GetAttributeValue<string>("syed_comment")));
                        solution["syed_status"] = "Completed";
                        CanPush = true;
                    }
                    catch (Exception ex)
                    {
                        solution["syed_status"] = "Error +" + ex.Message;
                    }
                    serviceProxy.Update(solution);
                }
            }
            return solutionFileInfos;
        }

        private SolutionFileInfo ExportSolution(OrganizationServiceProxy serviceProxy, string solutionUnqiueName, string owner, string message)
        {
            string filename;
            ExportSolutionRequest exportRequest = new ExportSolutionRequest();
            exportRequest.Managed = true;
            exportRequest.SolutionName = solutionUnqiueName;

            Console.WriteLine("Downloading Solutions");
            ExportSolutionResponse exportResponse = (ExportSolutionResponse)serviceProxy.Execute(exportRequest);

            //Handles the response
            byte[] downloadedSolutionFile = exportResponse.ExportSolutionFile;
            filename = solutionUnqiueName + "_" + ".zip";
            File.WriteAllBytes(RepositoryLocalFolder + filename, downloadedSolutionFile);

            SolutionFileInfo solutionFile = new SolutionFileInfo();
            solutionFile.SolutionFilePath = RepositoryLocalFolder + filename;
            solutionFile.OwnerName = owner;
            solutionFile.Message = message;
            solutionFile.SolutionUniqueName = solutionUnqiueName;
            Console.WriteLine("Solution Successfully Exported to {0}", filename);

            return solutionFile;
        }
    }

    internal interface ICrmSolutionHelper
    {
        string RepositoryUrl { get; set; }
        string Branch { get; set; }
        string OrganizationServiceUri { get; set; }

        bool CanPush { get; set; }

        List<SolutionFileInfo> SolutionFileInfos { get; set; }

        List<SolutionFileInfo> DownloadSolutionFile(string solutionUnqiueName, string owner, string message);
    }
}