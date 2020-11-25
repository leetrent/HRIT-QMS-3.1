using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using QmsCore.Model;

namespace QmsCore.Engine
{
    public class DataLoader
    {
        public IList Records;

        List<PropertyInfo> properties;

        string[] Rows;

        TableMap tableMap;
        public DataLoader()
        {
            Logger.Log.Record("DataLoader Initialized");
        }

        public void Load(TableMap map, string[] rows)
        {
            Logger.Log.Record(string.Format("Map for {0} loaded", map.ModelName));
            tableMap = map;
            Logger.Log.Record(string.Format("CSV File {0} loaded with {1} rows", map.CSVFileName,  rows.Length));
            Rows = rows;
            Logger.Log.Record("Begin call to SerializeObjects");
            SerializeObjects();
            Logger.Log.Record("Call to SerializeObjects completed");
        }       

        IList getList(Type t)
        {
            Logger.Log.Record("Begin to create IList of " + t.Name);
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(t);
            var instance = (IList)Activator.CreateInstance(constructedListType);
            Logger.Log.Record("Create IList of " + t.Name + " created.");
            return instance;
        }

        void setProperties( Type type)
        {
            properties = type.GetProperties().ToList();
        }

        PropertyInfo GetProperty(string propertyName)
        {
            return properties.Where(p => p.Name == propertyName).SingleOrDefault();
        }

        private void SerializeObjects()
        {

            Type type = Type.GetType(tableMap.ModelName + ", Qms_Data");
            setProperties(type);
            Logger.Log.Record("Beginning serialization of records to " + tableMap.ModelName);
            Records = getList(type);
            int counter = 0;
            int startIndex = 0;
            if(tableMap.HasHeader)
                startIndex = 1;

            int totalRows = Rows.Length;

            Logger.Log.Record(string.Format("{0} of {1} rows loaded",counter,totalRows));
            
            for(int i = startIndex; i < Rows.Length; i++) 
            {
                dynamic t = Activator.CreateInstance(type,new object[] {});
                string[] data = Rows[i].Split(tableMap.Delimiter);
          
                string initValue = string.Empty;                
                foreach(Column column in tableMap.Columns)
                {
                    try
                    {
                        PropertyInfo property = GetProperty(column.TableFieldname);
                        initValue = data[column.Index].Replace("\"","").Trim();
                        property.SetValue(t,getValue(column,initValue));
                    }
                    catch (System.Exception x)
                    {
                        Logger.Log.Record(LogType.Error,x.ToString());
                    }                    
                }
                Records.Add(t);
                counter++;
                if((counter % 100)==0)
                {
                    Logger.Log.Record(string.Format("{0} of {1} rows loaded",counter,totalRows));
                }    
            }
        }
        
        private object getValue(Column c, string initValue)
        {
            object retval = new object();
            try
            {
                if(!string.IsNullOrEmpty(initValue))
                {
                    switch(c.DataType.ToLower())
                    {
                        case "string":
                            retval = initValue;
                            break;
                        case "date":
                        case "datetime":
                            retval = DateTime.Parse(initValue);
                            break;
                        case "int":
                            retval = int.Parse(initValue);
                            break;
                        case "decimal":
                            retval = decimal.Parse(initValue);
                            break;
                        default:
                            retval = initValue;
                            break;
                    }//end switch
                }
                else
                {
                    retval = initValue;
                }

            }
            catch (System.Exception x)
            {
                Logger.Log.Record(LogType.Error, x.ToString());
            }
            return (object)retval;
        }


    }//end class
}//end namespace