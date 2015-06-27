using System.Configuration;

namespace Tarro
{
    internal class ServerSettings : ConfigurationSection
    {
        private static readonly ServerSettings settings
          = ConfigurationManager.GetSection("Tarro") as ServerSettings;

        public static ServerSettings Settings
        {
            get
            {
                return settings;
            }
        }

        [ConfigurationProperty("pathToApp", IsRequired = true)]
        public string PathToApp
        {
            get { return (string)this["pathToApp"]; }
            set { this["pathToApp"] = value; }
        }


        [ConfigurationProperty("executable", IsRequired = true)]
        public string Executable
        {
            get { return (string)this["executable"]; }
            set { this["executable"] = value; }
        }
        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }
        [ConfigurationProperty("managementPort",DefaultValue = 80, IsRequired = false)]
        public int ManagementPort
        {
            get { return (int)this["managementPort"]; }
            set { this["managementPort"] = value; }
        }
    }
}
