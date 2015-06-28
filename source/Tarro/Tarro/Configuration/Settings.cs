using System.Configuration;

namespace Tarro.Configuration
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
      
        [ConfigurationProperty("managementPort",DefaultValue = 2250, IsRequired = false)]
        public int ManagementPort
        {
            get { return (int)this["managementPort"]; }
            set { this["managementPort"] = value; }
        }
        [ConfigurationProperty("applications", IsRequired = false)]
        public ApplicationCollection Applications
        {
            get { return (ApplicationCollection)this["applications"]; }
        }
    }
}
