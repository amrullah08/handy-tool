using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmuCsvCon
{
    public class UserRegistration
    {
        public static void AddRegisteredUsers(List<Hacker> hacker)
        {
            DateTime registeredUsersStartTime = DateTime.Now;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            Console.WriteLine("Starting Adding Registered Users " + registeredUsersStartTime);
            try
            {
                int i = 0;
                foreach (var k in hacker)
                {
                    //while (j++ != 5660) continue;
                    Console.WriteLine("Adding / Updating user " + (i++).ToString() + k.alias);
                    //dict.Add((j++).ToString(), k.Fields[aliasIndex]);
                    try
                    {
                        using (SqlConnection con = new SqlConnection(Program.connectionString))
                        {
                            con.Open();
                            using (SqlCommand com = new SqlCommand("InsertUsers", con))
                            {
                                com.CommandType = System.Data.CommandType.StoredProcedure;
                                com.Parameters.Add(new SqlParameter("@Alias", k.alias)); //aliasIndex]));
                                com.Parameters.Add(new SqlParameter("@Name", k.name));// GetDisplayableName(firstIndex, k) + " " + GetDisplayableName(mIIndex, k) + " " + GetDisplayableName(lastIndex, k)));
                                com.Parameters.Add(new SqlParameter("@RegisteredOn", k.Registered_On));
                                com.Parameters.Add(new SqlParameter("@country", k.country));
                                com.Parameters.Add(new SqlParameter("@city", k.city));
                                com.Parameters.Add(new SqlParameter("@officelocation", k.officeLocation));
                                com.Parameters.Add(new SqlParameter("@location", k.campus));
                                com.Parameters.Add(new SqlParameter("@Upn", k.Upn));
                                com.ExecuteNonQuery();
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
            Console.WriteLine("Conversion Completed " + (DateTime.Now - registeredUsersStartTime));
        }
    }
}
