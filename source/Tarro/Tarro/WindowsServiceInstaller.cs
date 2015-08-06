using System;
using System.Collections;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using Tarro.Logging;

namespace Tarro
{
    internal class WindowsServiceInstaller
    {
        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller serviceProcessInstaller;

        public WindowsServiceInstaller(string serviceName, ServiceAccount account)
            : this(serviceName, account, ServiceStartMode.Manual)
        {

        }

        public WindowsServiceInstaller(string serviceName, ServiceAccount account, ServiceStartMode startmode)
            : this(serviceName, null, null, account, startmode)
        {
        }

        public WindowsServiceInstaller(string servicename, string username, string password, ServiceAccount account, ServiceStartMode startmode)
        {
            CreateServiceProcessInstaller(username, password, account);
            CreateServiceInstaller(servicename, startmode);
        }

        private void CreateServiceInstaller(string servicename, ServiceStartMode startMode)
        {
            serviceInstaller = new ServiceInstaller { ServiceName = servicename, StartType = startMode };
        }

        public void CreateServiceProcessInstaller(string username, string password, ServiceAccount account)
        {
            serviceProcessInstaller = new ServiceProcessInstaller
            {
                Account = account,
                Password = password,
                Username = username
            };
        }

        public void Install(IDictionary stateSaver)
        {
            var transactedInstaller = new TransactedInstaller();
            transactedInstaller.Installers.AddRange(new Installer[] { serviceProcessInstaller, serviceInstaller });


            Assembly assembly = Assembly.GetEntryAssembly();


            string path = string.Format("/assemblypath={0}", assembly.Location);
            string[] commandLine = { path };
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var context = new InstallContext(null, commandLine);
            transactedInstaller.Context = context;
            transactedInstaller.Install(stateSaver);
            InitializeEventSource();

        }

        private void InitializeEventSource()
        {
            LogSinks.EventLog().WriteEntry("Installed service");
        }

        public void Uninstall(IDictionary stateSaver)
        {
            var transactedInstaller = new TransactedInstaller();
            transactedInstaller.Installers.AddRange(new Installer[] { serviceProcessInstaller, serviceInstaller });

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            transactedInstaller.Uninstall(stateSaver);

        }


    }
}
