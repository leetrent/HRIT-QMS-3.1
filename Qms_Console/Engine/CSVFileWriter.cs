using System;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;

namespace QmsCore.Engine
{
    public class CSVWriter
    {
        TableMap tableMap;
        string header;
        public CSVWriter(TableMap map)
        {
            tableMap = map;
            header = string.Empty;
        }

        public void CreateFile()
        {
            string sql = getSqlStatement();
            string body = getCsvBody(sql);
            string csvFileName = Config.Settings.CSVDirectory + tableMap.CSVFileName;
            if(File.Exists(csvFileName))
                File.Delete(csvFileName);

            File.WriteAllText(csvFileName, body);

        }

        private string getSqlStatement()
        {
            tableMap.Columns.Sort();
            StringBuilder sb = new StringBuilder();
            sb.Append("Select ");
            int lastCount = tableMap.Columns.Count - 1;
            for(int i = 0; i < tableMap.Columns.Count; i++)
            {
                if(i < lastCount)
                {
                    header += tableMap.Columns[i].TableFieldname + ",";

                }
                else{
                    header += tableMap.Columns[i].TableFieldname;
                }
            }
            sb.Append(header);
            sb.Append(" from " + tableMap.TableName);
            return sb.ToString();
        }

        private string getCsvBody(string sql)
        {
            StringBuilder sb = new StringBuilder();
            if(tableMap.HasHeader)
                sb.AppendLine(header);

            using(MySqlConnection connection = new MySqlConnection(Config.Settings.ConnectionString))
            {
                connection.Open();
                using(MySqlCommand command = new MySqlCommand(sql,connection))
                {
                    MySqlDataReader reader = command.ExecuteReader();
                    int columnCount = tableMap.Columns.Count;
                    while(reader.Read())
                    {
                        string temp = string.Empty;
                        for(int i = 0; i < columnCount; i++)
                        {
                            if(!reader.IsDBNull(i))
                            {
                                temp += reader[i].ToString() + tableMap.Delimiter;
                            }
                            else
                            {
                                temp +=  tableMap.Delimiter;
                            }
                        }
                        sb.AppendLine(temp.Substring(0,temp.Length-1)); //remove final trailing comma
                    }
                }//end command
                connection.Close();
            }//end connection
            return sb.ToString();
        }


    }//end class
}//end namespace