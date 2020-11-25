using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.IO;

namespace QmsCore.Model
{
    public class TaskCleaner
    {
        public void CleanTasks()
        {
            string commandName = "aca.sp_QMS_CleanUpTasks";
            using(MySqlConnection connection = new MySqlConnection(Config.Settings.ReconDB))
            {
                using(MySqlCommand command = new MySqlCommand(commandName,connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    catch (System.Exception x)
                    {
                        throw x;
                    }
                    finally
                    {
                        if(connection.State == System.Data.ConnectionState.Open)
                            connection.Close();
                    }
                }//end command
            }//end connection            
        }//end CleanTasks
    }//end class
}//end namespace