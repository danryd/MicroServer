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
        [ConfigurationProperty("instanceName", IsRequired = false)]
        public string InstanceName
        {
            get { return (string)this["instanceName"]; }
            set { this["instanceName"] = value; }
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
