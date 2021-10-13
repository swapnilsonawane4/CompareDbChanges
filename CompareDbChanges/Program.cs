using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CompareDbChanges
{
    class Program
    {
        readonly static string fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"tableData.txt");
        readonly static string resultPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Result.txt");
        
        static void Main(string[] args)
        {
            List<string> tableName = new List<string>();
            
            string sourceConnString = "server=.;database=FM_Master;Integrated Security=True;MultipleActiveResultSets=true;";
            string destConnString = "server=.;database=FM_Hti_Local;Integrated Security=True;MultipleActiveResultSets=true;";
            
            tableName = Common.getTableList(sourceConnString); 
            createTextFile(tableName,sourceConnString, fullPath);
            compare(destConnString, fullPath, resultPath);
        }

        public static void checkColumns(string tableName,string columnName, string dataType, string connString,string resultPath)
        {
            //connection get columnName,dtype
            string cmd = "select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" + tableName + "'";
            DataTable dt = new DataTable();
            dt = Common.getDataTable(connString, cmd);

            int flag = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {   
                if (columnName == dt.Rows[i][3].ToString())
                {
                    //present column
                    Common.Write(resultPath, "Present Columns - {0}.\n", columnName);
                    if (dataType == dt.Rows[i][7].ToString())
                    {
                        //present datatype
                        Common.Write(resultPath, "Present DataType - {0}.\n", dataType);
                        flag = 1;
                    }
                    else
                    {
                        //not present datatype
                        Common.Write(resultPath, "Not Present DataType - {0}.\n", dataType);
                    }
                }
                
            }
            if(flag == 0)
            {
                //not present column
                Common.Write(resultPath, "Not Present Columns - {0}.\n", columnName);
            }
        }
        public static void compare(string destConnString,string fullPath,string resultPath)
        {
            File.WriteAllText(resultPath, "");
            string readText = File.ReadAllText(fullPath);

            //get data from sql
            List<string> tableName = new List<string>();
            tableName = Common.getTableList(destConnString);

            //compare
            string[] data = readText.Split("||");
            var tbname = "";
            var cname = "";
            var dataType = "";
            foreach (string d in data)
            {
                string[] data2 = d.Split("|");
                foreach (string k in data2)
                {
                    string[] data3 = k.Split(",");
                    if ((data3 != null) && (data3[0] != ""))
                    {
                        tbname = data3[1];
                    }
                }
                var result = tableName.FirstOrDefault(x => x == tbname);
                if (result != null)
                {
                    Common.Write(resultPath, "Present Tables - {0}.\n", tbname);
                    foreach (string k in data2)
                    {
                        string[] data3 = k.Split(",");
                        if ((data3 != null) && (data3[0] != ""))
                        {
                            tbname = data3[1];
                            cname = data3[2];
                            dataType = data3[3];
                            
                        }
                        checkColumns(tbname, cname, dataType, destConnString,resultPath);

                    }
                    

                }
                else
                {
                    Common.Write(resultPath, "Not Present Tables - {0}.\n", tbname);
                }

            }
            //log

        }
        public static void createTextFile(List<string> tableName,string connString,string fullPath)
        {
            File.WriteAllText(fullPath, "");
            foreach (var tName in tableName)
            {
                string cmd = "select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" + tName + "'";
                DataTable dt = new DataTable();
                dt = Common.getDataTable(connString, cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    using (StreamWriter writer = new FileInfo(fullPath).AppendText())
                    {
                        writer.Write("{0},{1},{2},{3}|", dt.Rows[i][0].ToString(), tName,dt.Rows[i][3].ToString(), dt.Rows[i][7].ToString());
                    }
                }
                Common.Write(fullPath, "|", null);
            }
        }
        
    }
}
