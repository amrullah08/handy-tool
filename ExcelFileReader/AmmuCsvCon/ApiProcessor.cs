using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmuCsvCon
{
    public class ApiProcessor
    {
        static string tempFileSheet1;
        static string tempFileSheet2;
        public static void Process()
        {
            try
            {
                string hackertxt = "E:\\excel\\hack" + Guid.NewGuid().ToString() + ".txt";
                string projecttxt = "E:\\excel\\project" + Guid.NewGuid().ToString() + ".txt";

                //GetHackersFromApi();       
                DateTime startTime = DateTime.Now;
                Console.WriteLine("Starting Conversion " + startTime);
                tempFileSheet1 = (Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())) + "data1";
                tempFileSheet2 = (Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())) + "data2";

                //ExcelToCSVCoversion(@"E:\excel\Input\AllReportJuly19PM.xlsx");
                Console.WriteLine("Conversion Completed " + (DateTime.Now - startTime));

                List<Hacker> hacker = null;
                List<Project> projects = null;

                Task addorUpdateusers = Task.Factory.StartNew(() => { hacker = Hacker.GetHackersFromApi(); }); //AddOrUpdateProject(projects);
                Task addorregisteredusers = Task.Factory.StartNew(() => { projects = Project.GetProjectForApi(); }); //AddRegisteredUsers(hacker);

                Task.WaitAll(addorUpdateusers, addorregisteredusers);


                //Task addorUpdateusers = Task.Factory.StartNew(() => { AddOrUpdateProject(projects); }); //AddOrUpdateProject(projects);
                //Task addorregisteredusers = Task.Factory.StartNew(() => { AddRegisteredUsers(hacker); }); //AddRegisteredUsers(hacker);

                File.WriteAllText(hackertxt, Newtonsoft.Json.JsonConvert.SerializeObject(hacker));
                File.WriteAllText(projecttxt, Newtonsoft.Json.JsonConvert.SerializeObject(projects));

                UserRegistration.AddRegisteredUsers(hacker);
                ProjectRegistration.AddOrUpdateProject(projects);

                //Task.WaitAll(addorUpdateusers, addorregisteredusers);

                Console.WriteLine("Entire Process Completed " + (DateTime.Now - startTime));

                Console.WriteLine("Start time " + DateTime.Now);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}
