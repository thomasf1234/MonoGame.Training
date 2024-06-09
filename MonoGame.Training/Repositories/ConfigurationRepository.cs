using MonoGame.Training.Models;
using Newtonsoft.Json;
using System.IO;

namespace MonoGame.Training.Repositories
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private const string _configPath = "config.json";
        public Configuration Load()
        {
            var configuration = Configuration.Instance;

            if (File.Exists(_configPath))
            {
                string configJson = File.ReadAllText(_configPath);
                var savedConfiguration = JsonConvert.DeserializeObject<Configuration>(configJson);

                configuration.Fullscreen = savedConfiguration.Fullscreen;
                configuration.RefreshRate = savedConfiguration.RefreshRate;
                configuration.Volume = savedConfiguration.Volume;
            }

            return configuration;
        }

        public void Save(Configuration configuration)
        {
            string json = JsonConvert.SerializeObject(configuration, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(_configPath, json);
        }
    }
}
