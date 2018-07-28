using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AmmuCsvCon
{
    public class ExcelProcessor
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

                ExcelToCSVCoversion(@"E:\excel\Input\20180725.xlsx");

                CsvHelper.CsvReader csvReader = new CsvHelper.CsvReader(File.OpenText(tempFileSheet1+".csv"));
                csvReader.Read();
                csvReader.ReadHeader();
                List<Hacker> hackers = new List<Hacker>();

                while (csvReader.Read())
                {
                    var hack = new Hacker();
                    hack.alias = csvReader.GetField("alias");
                    hack.Upn = csvReader.GetField("email");
                    hack.city = csvReader.GetField("hb_city");
                    hack.country = csvReader.GetField("hb_country");
                    hack.name = csvReader.GetField("PreferredFirstName");
                    hack.officeLocation = csvReader.GetField("Bldg").Replace("N/A","");
                    hack.Registered_On = csvReader.GetField<DateTime>("registration_date");
                    hackers.Add(hack);
                }

                csvReader = new CsvHelper.CsvReader(File.OpenText(tempFileSheet2 + ".csv"));
                csvReader.Read();
                csvReader.ReadHeader();
                List<Project> projects = new List<Project>();

                while (csvReader.Read())
                {
                    var hack = new Project();
                    hack.id = csvReader.GetField<int>("id");
                    hack.hackathon_name = csvReader.GetField("hackathon_name");
                    hack.description = csvReader.GetField("description");
                    hack.members = csvReader.GetField("members");
                    hack.tagline = csvReader.GetField("tagline");
                    hack.title = csvReader.GetField("title");
                    hack.venue = csvReader.GetField("venue");
                    projects.Add(hack);
                }

                Console.WriteLine("Conversion Completed " + (DateTime.Now - startTime));
                List<int> pids = new List<int>();
                pids.Add(70766);
                pids.Add(72814);
                pids.Add(72117);
                projects = projects.Where(cc => pids.Contains(cc.id)).ToList();
                
                //Task addorUpdateusers = Task.Factory.StartNew(() => { AddOrUpdateProject(projects); }); //AddOrUpdateProject(projects);
                //Task addorregisteredusers = Task.Factory.StartNew(() => { AddRegisteredUsers(hacker); }); //AddRegisteredUsers(hacker);

                File.WriteAllText(hackertxt, Newtonsoft.Json.JsonConvert.SerializeObject(hackers));
                File.WriteAllText(projecttxt, Newtonsoft.Json.JsonConvert.SerializeObject(projects));

                UserRegistration.AddRegisteredUsers(hackers);
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
        static void ExcelToCSVCoversion(string sourceFile)
        {
            Microsoft.Office.Interop.Excel.Application rawData = new Microsoft.Office.Interop.Excel.Application();
            string targetFile = string.Empty;
            try
            {
                Workbook workbook = rawData.Workbooks.Open(sourceFile);
                int i = 0;
                foreach (Worksheet sheet in workbook.Worksheets)
                {
                    if (i == 0)
                    {
                        sheet.SaveAs(tempFileSheet1, XlFileFormat.xlCSV);
                    }
                    else
                    {
                        if (i == 2)
                        {
                            try
                            {
                                sheet.SaveAs(tempFileSheet2, XlFileFormat.xlCSV);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    i++;

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                }
            }

            finally
            {
                rawData.DisplayAlerts = false;
                rawData.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rawData);
            }


            Console.WriteLine();
            Console.WriteLine($"The excel file {sourceFile} has been converted into {targetFile} (CSV format).");
            Console.WriteLine();
        }

    }
}
