using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastWayExcelReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Excel excel = new Excel(@"C:\Users\sadha\Pictures\BRV - DHL New Rates 19 Effect From March 2018.xlsx", 2);

            Zoneinfo zone = new Zoneinfo();

            zone.Countryname = excel.ReadCell(2, 0).ToString();
            zone.Countrycode = excel.ReadCell(2, 1);
            zone.zone = excel.ReadCell(2, 2);
            Console.WriteLine("countryname = {0} countrycode={1} code = {2}", zone.Countryname, zone.Countrycode, zone.zone);
        }
    }
}
