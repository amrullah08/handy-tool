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

            Excel exnew = new Excel(@"C:\Users\sadha\Pictures\BRV - DHL New Rates 19 Effect From March 2018.xlsx", 1);

            List<PriceInfo> priceinfolist = new List<PriceInfo>();

            for(int i = 6; i < 10; i++)
            {
                for(int j = 1; j <= 14; j++)
                {
                    PriceInfo pricein = new PriceInfo();
                    pricein.weight = exnew.ReadCell(i, 0);
                    pricein.price = exnew.ReadCell(i, j);
                    pricein.zone = j.ToString();
                    priceinfolist.Add(pricein);
                }
            }


            for (int i = 12; i < 61; i++)
            {
                for (int j = 1; j <= 14; j++)
                {
                    PriceInfo pricein = new PriceInfo();
                    pricein.weight = exnew.ReadCell(i, 0);
                    pricein.price = exnew.ReadCell(i, j);
                    pricein.zone = j.ToString();
                    priceinfolist.Add(pricein);
                }
            }

            for (int i = 64; i < 69; i++)
            {
                for (int j = 1; j <= 14; j++)
                {
                    PriceInfo pricein = new PriceInfo();
                    pricein.weight = exnew.ReadCell(i, 0);
                    pricein.price = exnew.ReadCell(i, j);
                    pricein.zone = j.ToString();
                    priceinfolist.Add(pricein);
                }
            }
            foreach(var PI in priceinfolist)
            {
                Console.WriteLine("weight = {0} zone = {1} price = {2}", PI.weight, PI.zone, PI.price);

            }
        }
    }
}
