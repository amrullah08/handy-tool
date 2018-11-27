using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FordPOCBot.Common
{
    public class Constants
    {

        public static double MaxQNAScore { get; set; } = Convert.ToDouble("60");
        public static double MaxLUIScore { get; set; } = Convert.ToDouble("0.6");
    }
}