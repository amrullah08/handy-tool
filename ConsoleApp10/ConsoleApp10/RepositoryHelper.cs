using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace ConsoleApp10
{
    internal class RepositoryHelper
    {
        public static void TryUpdateToRepository(string solutionUniqueName, string committerName, string committerEmail, string authorEmail)
        {
            ICrmSolutionHelper crmSolutionHelper = new CrmSolutionHelper(
                            ConfigurationManager.AppSettings["RepositoryUrl"],
                            ConfigurationManager.AppSettings["RepositoryLocalDirectory"],
                            ConfigurationManager.AppSettings["BranchName"],
                            ConfigurationManager.AppSettings["OrgServiceUrl"],
                            ConfigurationManager.AppSettings["UserName"],
                            ConfigurationManager.AppSettings["Password"]
                            );
            int timeOut = Convert.ToInt32(ConfigurationManager.AppSettings["SleepTimeoutInMillis"]);
            while (true)
            {
                HashSet<string> hashSet = new HashSet<string>();

                try
                {
                    var solutionFiles = crmSolutionHelper.DownloadSolutionFile(solutionUniqueName, null, null);

                    if (!crmSolutionHelper.CanPush)
                    {
                        System.Threading.Thread.Sleep(timeOut);
                        continue;
                    }

                    string solutionFilePath = ConfigurationManager.AppSettings["RepositoryLocalDirectory"] + "solutions.txt";
                    PopulateHashset(solutionFilePath, hashSet);

                    foreach (var solutionFile in solutionFiles)
                    {
                        if (!hashSet.Contains(solutionFile.SolutionUniqueName))
                            hashSet.Add(solutionFile.SolutionUniqueName);
                        SaveHashSet(solutionFilePath, hashSet);
                        TryPushToRepository(committerName, committerEmail, authorEmail, solutionFile, solutionFilePath);
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    Console.ReadLine();
                }
                System.Threading.Thread.Sleep(timeOut);
            }
        }

        private static void TryPushToRepository(string committerName, string committerEmail, string authorEmail,
            SolutionFileInfo solutionFile, string solutionFilePath)
        {
            GitDeploy.GitRepositoryManager gitRepositoryManager = new GitDeploy.GitRepositoryManager(
                                        ConfigurationManager.AppSettings["UserName"],
                                        ConfigurationManager.AppSettings["GitPassword"],
                                        ConfigurationManager.AppSettings["RepositoryUrl"],
                                        ConfigurationManager.AppSettings["RepositoryLocalDirectory"],
                                        solutionFile.OwnerName ?? committerName, authorEmail, committerName, committerEmail);

            //GitDeploy.GitRepositoryManager gitRepositoryManager = new GitDeploy.GitRepositoryManager(
            //    ConfigurationManager.AppSettings["UserName"],
            //    ConfigurationManager.AppSettings["GitPassword"],
            //    ConfigurationManager.AppSettings["RepositoryUrl"],
            //    ConfigurationManager.AppSettings["RepositoryLocalDirectory"],
            //    authorName, authorEmail, committerName, committerEmail);

            gitRepositoryManager.CommitAllChanges(solutionFile.Message, solutionFile.SolutionFilePath, solutionFilePath);
            gitRepositoryManager.PushCommits(ConfigurationManager.AppSettings["RemoteName"],
                ConfigurationManager.AppSettings["BranchName"]);
        }

        private static void PopulateHashset(string solutionFilePath, HashSet<string> hashSet)
        {
            if (File.Exists(solutionFilePath))
            {
                string[] lines = File.ReadAllLines(solutionFilePath);
                foreach (var line in lines)
                {
                    hashSet.Add(line);
                }
            }
            else
            {
                File.Create(solutionFilePath);
            }

        }

        private static void SaveHashSet(string solutionFilePath, HashSet<string> hashSet)
        {
            File.WriteAllText(solutionFilePath, string.Empty);
            File.WriteAllLines(solutionFilePath, hashSet.ToArray());
        }
    }
}