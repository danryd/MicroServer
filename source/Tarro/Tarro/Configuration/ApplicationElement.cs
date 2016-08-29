using System.Configuration;

namespace Tarro.Configuration
{
    internal class ApplicationElement : ConfigurationElement
    {

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
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

        [ConfigurationProperty("runMode", IsRequired = false, DefaultValue = Tarro.RunMode.AppDomain)]
        public RunMode RunMode
        {
            get { return (RunMode)this["runMode"]; }
            set { this["runMode"] = value; }
        }
    }
}