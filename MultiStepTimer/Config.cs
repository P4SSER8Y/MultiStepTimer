using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace MultiStepTimer
{
    public class ConfigItem
    {
        public string title { get; set; }
        public double timeout { get; set; }
    }

    public class Config
    {
        public List<ConfigItem> items { get; set; }
    }

    class ConfigReader
    {
        static public Config Read(string filename)
        {
            StreamReader sr = new StreamReader(filename, Encoding.UTF8);
            var content = sr.ReadToEnd();
            sr.Close();

            var input = new StringReader(content);
            var deserializer = new Deserializer();
            var ret = deserializer.Deserialize<Config>(input);

            foreach (var item in ret.items)
                Console.WriteLine($"{item.title}: {item.timeout}");
            return ret;
        }
    }
}
