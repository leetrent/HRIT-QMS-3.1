using System;
using System.IO;
using System.Reflection;
namespace QmsCore.Engine
{
    public sealed class Logger
    {
        private static readonly Logger log = new Logger();
        private static string logFileName = Config.Settings.LogDirectory + DateTime.Now.ToString("yyyy-MM-dd") + "_QmsReconLog.txt";
        public static Logger Log
        {
            get{return log;}
        }

        



/*
         public void RecordAssemblyInfo(string method)
        {
            try
            {
                AppDomain MyDomain = AppDomain.CurrentDomain;
                Assembly[] AssembliesLoaded = MyDomain.GetAssemblies();
                string message = string.Empty;
                int i = 0;
                foreach (Assembly assembly in AssembliesLoaded)
                {
                    if(assembly.FullName.StartsWith("Qms_Data"))
                    {
                        i++;
                        message+="\r\n" + i + ") " + assembly.FullName;
                    }
                } 
                Record(method + message );
                            
            }
            catch (System.Exception)
            {
                throw;
            }

        } 
 */
        public void Record(string entry)
        {
            Record(LogType.Status,entry);
        }

        public void Record(LogType logType, string entry)
        {
            string ts = DateTime.Now.ToLongTimeString();
            string lType = string.Empty;
            if(logType == LogType.Error)
            {
                lType = "Error";
            }
            else
            {
                lType = "Status";
                System.Console.WriteLine(ts + " " + entry);
            }
            string message = string.Format("{0}\t{1}\t{2}",ts,lType,entry);
            using(StreamWriter streamWriter = new StreamWriter(logFileName,true))
            {
                try
                {
                    streamWriter.WriteLineAsync(message);
                    streamWriter.Close();
                }
                catch (System.Exception)
                {
                    //throw;
                }
            }

        }
        
    }//end class
}//end namespace