using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoAPServiceApplication
{
    public class DataEntry
    {
        public int channel { get; set; }
        public DateTime dateTime { get; set; }
        public String units { get; set; }
        public String channelName { get; set; }
        public double value { get; set; }
        public int status { get; set; }
        //private int saveCol;
    }
}