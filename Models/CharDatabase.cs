using System.Collections.Generic;

namespace CatalogueDownloader.Models
{
    class CharDatabase
    {
        public List<Characteristic> charList{ get; set; }

        public void Add(Characteristic charac)
        {
            this.charList.Add(charac);
        }

        public void AddDic(Dictionary<string, string> dic)
        {
            foreach (var entry in dic)
            {
                var charac = new Characteristic(entry.Key, entry.Value);
                this.charList.Add(charac);
            }
        }

        public CharDatabase()
        {
            this.charList = new List<Characteristic>();
        }
    }
}
