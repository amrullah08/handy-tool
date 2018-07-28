using Adx.Migration.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmuCsvCon
{
    public class Hacker : ILocation
    {
        static string userUrl;
        public string name { get; set; }
        public string alias { get; set; }
        public DateTime Registered_On { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string officeLocation { get; set; }

        public string campus
        {
            get
            {
                return this.officeLocation.Split('/')[0];
            }
        }
        public string Upn { get; set; }
        static List<Hacker> GetHackers(dynamic data)
        {
            var hackers = new List<Hacker>();
            foreach (var d in (data as JArray))
            {
                hackers.Add(GetHacker(d));
            }
            return hackers;
        }


        static Hacker GetHacker(JToken data)
        {
            return new Hacker
            {
                alias = (string)data["alias"],
                Registered_On = (DateTime)data["joined_at"],
                name = (string)data["name"],
                city = (string)data["city"],
                country = (string)data["country"],
                officeLocation = data["meta"].ToObject<meta>().location,
                Upn = data["meta"].ToObject<meta>().upn
            };
        }
        public static List<Hacker> GetHackersFromApi()
        {

            var cutoffDate = DateTime.Parse("2018-07-18T00:00:00.000Z");
            var hackers = new List<Hacker>();
            var client = new HttpClientHelper();

            var offset = 0;
            var limit = 1000;
            var max = 16594;
            while (offset < max)

            {
                try
                {
                    var url = string.Format(userUrl, offset, limit);
                    var data = client.Get<dynamic>(new Uri(url), Program.token);
                    List<Hacker> k = Hacker.GetHackers(data.data);
                    if (k == null || k.Count == 0)
                        break;
                    hackers.AddRange(Hacker.GetHackers(data.data));
                    offset += limit;

                }
                catch (Exception ex)
                {
                    break;
                }
                offset++;
            }
            return hackers;
        }
    }
}
