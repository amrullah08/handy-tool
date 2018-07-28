using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmmuCsvCon
{
    internal interface ILocation
    {
        string city { get; set; }
        string country { get; set; }
        string officeLocation { get; set; }

        string campus { get; }
    }
}
