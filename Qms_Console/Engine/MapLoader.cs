using Newtonsoft.Json;

namespace QmsCore.Engine
{
    public class MapLoader
    {
        public TableMap Map {get;set;}

        public MapLoader(string mappingFile)
        {
            loadMap(mappingFile);
        }

        private void loadMap(string mappingFile)
        {
            string json = System.IO.File.ReadAllText(mappingFile);
            Map = JsonConvert.DeserializeObject<TableMap>(json);
        }




    }//end class
}//end namespace