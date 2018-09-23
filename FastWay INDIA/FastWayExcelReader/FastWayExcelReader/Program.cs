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

           
            List<Zoneinfo> zoneinfolist = new List<Zoneinfo>();

           

            for(int i = 2; i < 118; i++)
            {
                Zoneinfo excelzone = new Zoneinfo();

                excelzone.Countryname = excel.ReadCell(i, 0).ToString();
                excelzone.Countrycode = excel.ReadCell(i, 1);
                excelzone.zone = excel.ReadCell(i, 2);

                zoneinfolist.Add(excelzone);
            }

            for(int j = 2; j < 118; j++)
            {
                Zoneinfo excelzone = new Zoneinfo();

                excelzone.Countryname = excel.ReadCell(j, 3).ToString();
                excelzone.Countrycode = excel.ReadCell(j, 4);
                excelzone.zone = excel.ReadCell(j, 5);

                zoneinfolist.Add(excelzone);
            }
            foreach (var zi in zoneinfolist)
            {
                Console.WriteLine("countryname = {0} countrycode = {1} code = {2}", zi.Countryname, zi.Countrycode, zi.zone);
            }
        }
    }
}
