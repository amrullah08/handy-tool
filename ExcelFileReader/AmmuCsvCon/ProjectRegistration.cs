using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmuCsvCon
{
    public class ProjectRegistration
    {
        public static void AddOrUpdateProject(List<Project> projects)
        {
            DateTime addOrUpdateProjectStartTime = DateTime.Now;
            Console.WriteLine("Starting Add or Update Project Information " + addOrUpdateProjectStartTime);
            try
            {
                int i = 0;
                foreach (var k in projects)
                {
                    Console.WriteLine("Adding / Updating Project and its members " + (i++).ToString() + k.title);
                    try
                    {
                        if (k.id == 73411)
                        {
                            Console.WriteLine("id");
                        }
                        using (SqlConnection con = new SqlConnection(Program.connectionString))
                        {
                            con.Open();
                            using (SqlCommand com = new SqlCommand("InsertOrUpdateProject", con))
                            {
                                com.CommandType = System.Data.CommandType.StoredProcedure;
                                com.Parameters.Add(new SqlParameter("@Id", k.id));
                                com.Parameters.Add(new SqlParameter("@Title", k.title));
                                com.Parameters.Add(new SqlParameter("@Description", k.description));
                                com.Parameters.Add(new SqlParameter("@TagLine", k.tagline));
                                com.Parameters.Add(new SqlParameter("@Members", k.members));
                                com.Parameters.Add(new SqlParameter("@Venue", k.venue));
                                com.Parameters.Add(new SqlParameter("@HackathonName", k.hackathon_name));

                                int j = com.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine("Add or Update Project Information Completed " + (DateTime.Now - addOrUpdateProjectStartTime));
        }

    }
}
