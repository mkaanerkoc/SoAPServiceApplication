using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoAPServiceApplication
{
    public class DataEntry
    {
        public int channelID { get; set; }
        public int saveCol { get; set; }
        public int status { get; set; }
        public double value { get; set; }
        public string channelName { get; set; }
        public string siteName { get; set; }
        public string units { get; set; }
        public DateTime Date_Time { get; set; }
    }
}