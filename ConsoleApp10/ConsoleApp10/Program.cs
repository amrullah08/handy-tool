using Octokit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp10
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                args = new string[] { "SyedAmrullahMazhar", "Syed Amrullah", "syamrull@microsoft.com" };
            }

            string solutionUniqueName = null; //args[0];
            string committerName = args[1];
            string committerEmail = args[2];


            string authorName = "Solution Committer Service";
            string authorEmail = "TestSolutionCommitterService@microsoft.com";
            RepositoryHelper.TryUpdateToRepository(solutionUniqueName, committerName, committerEmail, authorEmail);

        }

    }
}
