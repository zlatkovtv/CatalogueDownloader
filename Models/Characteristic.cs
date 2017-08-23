using System.Linq.Expressions;

namespace CatalogueDownloader.Models
{
    class Characteristic
    {
        public string CharName { get; set; }
        public string CharValue { get; set; }

        public Characteristic(string key, string value)
        {
            this.CharName = key;
            this.CharValue = value;
        }

        public static Characteristic ParseStrToCharacteristic(string input)
        {
            string key = input.Split(':')[0];
            string value = input.Split(':')[0];
            var charac = new Characteristic(key, value);
            return charac;
        }

        public override string ToString()
        {
            return CharName + ":" + CharValue;
        }
    }
}
