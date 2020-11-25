using System;
using System.IO;
using QmsCore.Model;
using QmsCore.Validation;
using MySql.Data.MySqlClient;

namespace QmsCore.Engine
{
    public class Core
    {
        

        public void Execute()
        {
            executeValidations();
            //executeTaskUpdates();
        }

#region Validations
        private void executeValidations()
        {
            string[] jsonFileNames = Directory.GetFiles(Config.Settings.JsonDirectory,"*.json");
            foreach(string jsonFileName in jsonFileNames)
            {
                try
                {
                    TableMap tableMap = getTableMap(jsonFileName);
                    string csvFile = Config.Settings.CSVDirectory + tableMap.CSVFileName;
                    if(tableMap.SourceType == "Table")
                    {
                        CSVWriter writer = new CSVWriter(tableMap);
                        writer.CreateFile();
                    }
                    string[] rows = loadCsvRows(csvFile, tableMap.HasHeader);   
                    DataLoader dataLoader = getDataLoader(tableMap, rows);
                    validate(dataLoader,tableMap);
                    archive(csvFile);                    
                }
                catch (System.Exception x)
                {
                    System.Console.WriteLine(x.ToString());
                }
            }
        }


        string[] loadCsvRows(string csvFileName, bool hasHeader)
        {
            CSVLoader csvLoader = new CSVLoader(hasHeader);
            csvLoader.ReadFile(csvFileName);
            return csvLoader.Records;
        }

        DataLoader getDataLoader(TableMap tableMap, string[] rows)
        {
            DataLoader dataLoader = new DataLoader();
            dataLoader.Load(tableMap,rows); 
            return dataLoader;    

        }

        TableMap getTableMap(string jsonFile)
        {
            MapLoader mapLoader = new MapLoader(jsonFile);
            TableMap tableMap = mapLoader.Map;
            return tableMap;
        }

        void validate(DataLoader dataLoader, TableMap tableMap)
        {
            IValidator val = getValidator(tableMap.Validator);
            val.Validate(dataLoader.Records, tableMap.UseSoftDelete);
            val.Save();
        }

        IValidator getValidator(string validatorType)
        {
            Type type = Type.GetType(validatorType);
            dynamic t = Activator.CreateInstance(type,new object[] {});
            Logger.Log.Record(t.ToString());
            return (IValidator)t;

        }

        void archive(string csvFile)
        {
            try
            {
                string dateTimeToAppend = DateTime.Now.ToString("yyyyMMdd");
                string csvArchivedFile = csvFile.Replace(Config.Settings.CSVDirectory, Config.Settings.ArchiveDirectory).Replace(".", "." + dateTimeToAppend + ".");
                if(System.IO.File.Exists(csvArchivedFile))
                    System.IO.File.Delete(csvArchivedFile);

                System.IO.File.Move(csvFile,csvArchivedFile);                
                
            }
            catch (System.Exception)
            {
//                throw;
            }
        }

#endregion        

#region CleanUpTasks
    private void executeTaskUpdates()
    {
        try
        {
            TaskCleaner taskCleaner = new TaskCleaner();
            taskCleaner.CleanTasks();
        }
        catch (System.Exception x)
        {
            Logger.Log.Record(LogType.Error,x.ToString());
        }
    }



#endregion



        
    }//end class
}//end namespace