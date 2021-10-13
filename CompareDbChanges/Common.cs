using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace CompareDbChanges
{
    class Common
    {
        public static List<string> getTableList(string connString)
        {
            string cmd = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'";

            DataTable dt = new DataTable();
            dt = getDataTable(connString, cmd);
            List<string> tableName = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                tableName.Add(dt.Rows[i][2].ToString());
            }
            return tableName;
        }
        public static DataTable getDataTable(string connectionString, string sqlCommand)
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(connectionString);
                SqlCommand cm = new SqlCommand(sqlCommand, con);
                con.Open();
                SqlDataReader sdr = cm.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(sdr);

                return dt;
            }
            catch (Exception e)
            {
                Console.WriteLine("OOPs, something went wrong." + e);
                throw;
            }
            finally
            {
                con.Close();
            }
        }
        public static void Write(string path, string firstParam, string secondParam)
        {
            using (StreamWriter writer = new FileInfo(path).AppendText())
            {
                writer.Write(firstParam, secondParam);
            }
        }
    }
}
