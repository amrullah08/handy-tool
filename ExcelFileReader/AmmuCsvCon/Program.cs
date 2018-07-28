using Adx.Migration.Services;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AmmuCsvCon
{
    class Program
    {
        public static string connectionString = "dbconnection string";
        public static string token = "aad token";

        static void Main(string[] args)
        {
            //ExcelProcessor.Process();
            ApiProcessor.Process();
        }

    }
}
