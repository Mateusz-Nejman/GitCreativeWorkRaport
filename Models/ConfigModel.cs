using System.IO;
using System.Text.Json;

namespace GitCreativeWorkRaport.Models
{
    internal class ConfigModel
    {
        public string Login { get; set; } = string.Empty;
        public List<string> RepoPaths { get; set; } = [];

        public void Save(string path)
        {
            string json = JsonSerializer.Serialize(this);
            File.WriteAllText(path, json);
        }

        public static ConfigModel Load(string path)
        {
            if (!File.Exists(path))
            {
                return new ConfigModel();
            }

            string json = File.ReadAllText(path);

            return JsonSerializer.Deserialize<ConfigModel>(json);
        }
    }
}
