using Adx.Migration.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmuCsvCon
{
    public class Project
    {
        public int id { get; set; }
        public string title { get; set; }
        public string tagline { get; set; }
        public string description { get; set; }

        public string members { get; set; }

        public string venue { get; set; }

        public string hackathon_name { get; set; }

        public static List<Project> GetProjectForApi()
        {
            var projectUrl = "https://hackbox-api.azurewebsites.net/hackathons/1214/projects?offset={0}&limit={1}&venue=%5B%22India%20-%20Bangalore%2C%20Ferns%22%2C%22India%20-%20Bangalore%2C%20Vigyan%22%2C%22India%20-%20Hyderabad%2C%20Main%20Campus%22%2C%22India%20-%20Hyderabad%22%5D";

            var cutoffDate = DateTime.Parse("2018-07-18T00:00:00.000Z");
            var hackers = new List<Project>();
            var client = new HttpClientHelper();

            var offset = 0;
            var limit = 1000;
            var max = 16594;
            while (offset < max)

            {
                try
                {
                    var url = string.Format(projectUrl, offset, limit);
                    var data = client.Get<dynamic>(new Uri(url), ConfigurationConstants.Token);
                    if (data != null)
                    {
                        var k = GetProjects(data.data);
                        if (k == null || k.Count == 0)
                            break;
                        hackers.AddRange(k);
                        offset += limit;
                    }
                }
                catch (Exception ex)
                {
                    break;
                }
            }
            return hackers;
        }

        private static List<Project> GetProjects(dynamic data)
        {
            var hackers = new List<Project>();
            foreach (var d in (data as JArray))
            {
                hackers.Add(GetProject(d));
            }
            return hackers;
        }
        private static Project GetProject(JToken data)
        {
            return new Project
            {
                id = (int)data["id"],
                title = (string)data["title"],
                tagline = (string)data["tagline"],
                description = (string)data["description"],
                venue = (string)data["venue"],
                members = GetMembers((data["members"].ToObject<List<Member>>())),
                hackathon_name = (string)data["hackathon_name"]
            };
        }

        private static string GetMembers(List<Member> members)
        {
            string s = string.Empty;
            foreach (var m in members)
            {
                s += m.alias + "|";
            }
            s.LastIndexOf("|");
            return s.Remove(s.Length - 1, 1);
        }

    }
}
