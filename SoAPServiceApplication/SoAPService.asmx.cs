using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Hosting;
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
        private static SqlConnection myConnection;
        private static List<string> tableNames;
        private static List<string> siteNames;
        private static List<Channel> channelList;
        private static List<DataEntry> dataEntryList;
        //const string  connectString =         "Data Source=LAPTOP-H740Q104;Initial Catalog=EnvidasUzunDere;Integrated Security=True";
        private const String ConnectionString = "Data Source=LAPTOP-H740Q104;Initial Catalog=EnvidasUzunDere;Integrated Security=True";

        [WebMethod]
        public List<DataEntry> Response()
        {
            channelList = new List<Channel>();
            dataEntryList = new List<DataEntry>();
            siteNames = new List<string>();
            tableNames = new List<string>();
            string cString = getConnectionString();
            System.Diagnostics.Debug.WriteLine("SomeText");
            ConnectSQL(cString);
            getSiteNames();
            getChannelsData();
            tableNames = GetTableNames("S0", "RAW", "01");
            dataEntryList.Clear();
            for (int i = 0; i < tableNames.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine(tableNames[i]);
                //Console.WriteLine("Table Names : {0}", tableNames[i]);
                ReadTable(tableNames[i], siteNames[i]);

            }
            return dataEntryList;
        }
        private static String getConnectionString()
        {
            string connectString = "";
            connectString = System.Configuration.ConfigurationManager.ConnectionStrings["IISConnectionString"].ToString();
            return connectString;
        }

        private static void ConnectSQL(string conString)
        {
            myConnection = new SqlConnection(conString);
            try
            {
                if (myConnection.State == ConnectionState.Closed)
                    myConnection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public static List<string> GetTableNames(string searchName, string nonSearchName, string timeBase)
        {
            System.Data.DataTable schema = myConnection.GetSchema("Tables");
            List<string> TableNames = new List<string>();
            foreach (System.Data.DataRow row in schema.Rows)
            {
                if (row[2].ToString().Contains(searchName) && !row[2].ToString().Contains(nonSearchName))
                {
                    if (row[2].ToString().Substring(5).Equals(timeBase))
                    {
                        TableNames.Add(row[2].ToString());
                    }
                }
            }
            return TableNames;
        }
        private static void getSiteNames()
        {

            siteNames.Clear();
            SqlDataReader myReader = null;
            SqlCommand myCommand = new SqlCommand("SELECT * FROM Site ORDER BY number", myConnection);
            myReader = myCommand.ExecuteReader();
            while (myReader.Read())
            {
                siteNames.Add(myReader["name"].ToString());
            }
            myReader.Close();
        }
        private static void ReadTable(string tableName, string siteName)
        {
            try
            {
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand("SELECT TOP 1 * FROM " + tableName + " ORDER BY Date_Time DESC;", myConnection);
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    DateTime logDateTime = (DateTime)myReader["Date_Time"];
                    System.Diagnostics.Debug.Write("Date time:  ");
                    System.Diagnostics.Debug.WriteLine(logDateTime);
                    for (int i = 0; i < myReader.FieldCount; i++)
                    {
                        if (myReader.GetName(i).Contains("Value"))
                        {
                            int channelID = Convert.ToInt16(myReader.GetName(i).Substring(5));
                            String valueColumnName = myReader.GetName(i);
                            String statusColumnName = "Status" + channelID.ToString();
                            Object valueObj = myReader[valueColumnName];
                            Object statusObj = myReader[statusColumnName];
                            //Console.WriteLine("Value Name : {0}", myReader.GetName(i));
                            System.Diagnostics.Debug.Write("Value Name : ");
                            System.Diagnostics.Debug.WriteLine(myReader.GetName(i));
                            DataEntry entry = new DataEntry();

                            Channel ch = findChannel(channelID);
                            if (!object.ReferenceEquals(null, ch))
                            {

                                //Console.WriteLine("Value {0}:  {1}", channelID, myReader.GetValue(i).ToString());
                                System.Diagnostics.Debug.Write("Channel Name : ");
                                System.Diagnostics.Debug.WriteLine(ch.name);
                                entry.channelName = ch.name;
                                //entry.state = (int)myReader[
                                entry.Date_Time = logDateTime;
                                entry.siteName = siteName;
                                entry.channelID = ch.channelID;
                                entry.units = ch.units;
                                entry.saveCol = ch.saveCol;
                                if (valueObj.GetType() != typeof(System.DBNull))
                                    entry.value = (double)valueObj;
                                else
                                    entry.value = -9999;
                                if (statusObj.GetType() != typeof(System.DBNull))
                                    entry.status = (int)statusObj;
                                else
                                    entry.status = -9999;

                                dataEntryList.Add(entry);
                            }
                        }
                    }
                }
                myConnection.Close();
                myReader.Close();
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        private static void getChannelsData()
        {
            SqlDataReader myReader = null;
            SqlCommand myCommand = new SqlCommand("select * from  Channel", myConnection);
            myReader = myCommand.ExecuteReader();
            channelList.Clear();
            while (myReader.Read())
            {
                Channel tChannel = new Channel();
                tChannel.channelID = (int)myReader["number"];
                tChannel.saveCol = (int)myReader["saveCol"];
                tChannel.units = (string)myReader["units"];
                tChannel.name = (string)myReader["name"];
                channelList.Add(tChannel);
                System.Diagnostics.Debug.Write("Channel Name : ");
                System.Diagnostics.Debug.WriteLine(tChannel.name);
                //Console.WriteLine("Channel Info : {0}--{1}", myReader["saveCol"], myReader["name"]);
            }
            myReader.Close();
        }

        private static Channel findChannel(int id)
        {
            Channel c =null;
            for (int i = 0; i < channelList.Count; i++)
            {
                if (id == channelList[i].saveCol)
                {
                    c = channelList[i];
                    break;
                }
            }
            return c;
        }

        private static void printDataEntryList()
        {
            for (int i = 0; i < dataEntryList.Count; i++)
            {
                //Console.WriteLine("Site Name : {0} , Channel Name : {1}, Channel ID : {5}, Value : {2}, Units : {3},DateTime : {4}", dataEntryList[i].channelName, dataEntryList[i].value, dataEntryList[i].units, "heeheh", dataEntryList[i].saveCol);
                Console.WriteLine("Site Name : {0} , Channel Name : {1} , Channel ID : {2}, DateTime : {3}", dataEntryList[i].siteName, dataEntryList[i].channelName, dataEntryList[i].saveCol, dataEntryList[i].Date_Time);
            }
        }
    
    }
    
}
