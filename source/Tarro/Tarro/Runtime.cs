using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using Tarro.Logging;

namespace Tarro
{
    internal class Runtime
    {
        private static readonly ILog log = LogFactory.GetLogger<Runtime>();

        public static void Execute()
        {
            string[] args = ExtractComandArgs();
            if (Environment.UserInteractive)
            {
                log.Info("User interactive mode");
                if (RunInstallation(args))
                {
                    ManageInstallation(args);
                }
                else
                {
                    Console.WriteLine("Press a key to shut down");
                    using (var application = new Application(ServerSettings.Settings.PathToApp, ServerSettings.Settings.Executable))
                    {
                        application.Start();
                        Console.ReadKey();
                    }

                }
            }
            else
            {
                log.Info("Service mode");
                ServiceBase.Run(new ApplicationService());
            }
        }

        private static bool RunInstallation(string[] args)
        {
            return args.Length > 0;
        }

        private static string[] ExtractComandArgs()
        {
            return Environment.GetCommandLineArgs().Skip(1).ToArray();
        }

        private static void ManageInstallation(string[] args)
        {
            if (args[0] == "-?")
            {
                PrintUsage();
            }
            var name = args[1];
            var installer = new WindowsServiceInstaller(name, ServiceAccount.NetworkService);
            if (args[0] == "install")
            {
                installer.Install(new Hashtable());
            }
            else if (args[0] == "uninstall")
            {
                installer.Uninstall(new Hashtable());
            }
            else
            {
                PrintUsage();
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine(
                @"To install as a service use
 {0} install [servicename]", Assembly.GetEntryAssembly().GetName().Name);
            Environment.Exit(0);
        }
    }
}
