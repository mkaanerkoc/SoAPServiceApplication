using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace SoAPServiceApplication
{
    /// <summary>
    /// Summary description for SoAPService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SoAPService : System.Web.Services.WebService
    {

        const string  connectString = "Data Source=LAPTOP-H740Q104;Initial Catalog=EnvidasUzunDere;Integrated Security=True";

        DataClasses1DataContext db = new DataClasses1DataContext(connectString);

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public List<DataEntry> Response()
        {
            List<DataEntry> dataList = new List<DataEntry>();
            var channelCount = db.Channels.Select(s => s).ToArray().Length;
            var datas = db.InstantDatas.OrderByDescending(s => s.Date_Time).Take(channelCount).ToArray();
            Array.Sort(datas, delegate (InstantData x, InstantData y) { return x.channel.CompareTo(y.channel); });
            for (int i = 0; i < datas.Length; i++)
            {
                DataEntry entry = new DataEntry();
                entry.channelName = datas[i].Channels.name;
                entry.value = datas[i].value;
                entry.channel = datas[i].channel;
                entry.dateTime = datas[i].Date_Time;
                entry.status = datas[i].status;
                entry.units = datas[i].Channels.units;
                dataList.Add(entry);
            }
            return dataList;
        }
    }
}
