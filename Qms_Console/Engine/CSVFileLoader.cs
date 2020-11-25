using System;
using System.IO;

namespace QmsCore.Engine
{
    public class CSVLoader
    {
        bool HasHeader {get;}

        public string[] Records {get;set;}

        public CSVLoader(bool hasHeader)
        {
            HasHeader = hasHeader; 
        }

        public void ReadFile(string fileName)
        {
            ReadFile(fileName,true);
        }

        public void ReadFile(string fileName, bool removeDoubleQuotes)
        {
            Logger.Log.Record("Loading CSV File: " + fileName);
            Records = loadRecords(fileName);
            Logger.Log.Record(Records.Length + " records loaded");
            if(removeDoubleQuotes)
            {
                cleanLines();
                Logger.Log.Record("Double quotes removed");
            }
        }


        string[] loadRecords(string fileName)
        {
            string[] retval = new string[]{};
            try
            {
                retval = File.ReadAllLines(fileName);
            }
            catch (System.Exception x)
            {
                Logger.Log.Record(LogType.Error, "Failed to read " + fileName + "\r\n" + x.ToString());
            }
            return retval;
        }

        void cleanLines()
        {
            string[] temp = new string[Records.Length];
            int startIndex = 0;

            if(HasHeader)
                startIndex = 1;


            for(int i = startIndex; i<Records.Length; i++)
            {
                temp[i] = Records[i].Replace("\"",string.Empty);
            }
            Records = temp;
        }


        

    }//end class
}//end namespace