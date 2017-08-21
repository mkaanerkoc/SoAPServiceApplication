using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoAPServiceApplication
{
    public class SiteEntry
    {
        public string siteName { get; set; }
        public int siteID { get; set; }
        public SiteEntry() {  }
        public SiteEntry(string stn,int sitid)
        {
            this.siteName = stn;
            this.siteID = sitid;
        }
    }
}