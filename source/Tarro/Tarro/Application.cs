using System;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using Tarro.Logging;

namespace Tarro
{
    enum RunMode
    {
        AppDomain, Process
    }
    internal class Application : IDisposable
    {

        private readonly ILog log = LogFactory.GetLogger<Application>();
        private readonly string name;
        private readonly string pathToApp;
        private readonly string executable;
        private readonly AppWatcher watcher;
        private readonly AppCopy appCopy;
        private Process process;
        private AppDomain domain;
        private readonly RunMode runMode;
        private readonly string cachePath = "appCache";
        public Application(string name, string pathToApp, string executable)
        {
            this.name = name;
            this.pathToApp = pathToApp;
            this.executable = executable;
            this.runMode = RunMode.AppDomain;

            appCopy = new AppCopy(cachePath, pathToApp, executable);
            watcher = new AppWatcher(pathToApp);
            watcher.AppChanged += (o, e) =>
            {
                Stop();
            };
            watcher.AfterQuietPeriod += (o, e) =>
            {
                Start();
            };
        }

        public void Start()
        {
            try
            {

                lock (appLock)
                {
                    if (RuntimeIsActive()) //Should be null if stopped
                        return;
                    log.Info("Starting application ({0})", name);
                    appCopy.ShadowCopy();
                    switch (runMode)
                    {
                        case RunMode.AppDomain:
                            StartAppdomain();
                            break;
                        case RunMode.Process:
                            StartProcess();
                            break;
                        default:
                            throw new InvalidOperationException("Unknown runmode");
                    }
                    log.Info("Application started ({0})", name);
                }
            }
            catch (Exception ex)
            {
                log.Error("Unable to start application ({0})", ex, name);
            }
        }

        private bool RuntimeIsActive()
        {
            return runMode == RunMode.AppDomain && domain != null || runMode == RunMode.Process && process != null;
        }


        private void StartAppdomain()
        {
            var setup = new AppDomainSetup();
            setup.ApplicationBase = pathToApp;

            setup.ApplicationBase = appCopy.ShadowPath;
            setup.PrivateBinPath = appCopy.ShadowPath;
            var potentialConfigFile = executable + ".config";
            if (File.Exists(Path.Combine(pathToApp, potentialConfigFile)))
                setup.ConfigurationFile = potentialConfigFile;
            domain = AppDomain.CreateDomain(executable, new Evidence(), setup);
            domain.ExecuteAssembly(GetExecutablePath());
            domain.DomainUnload += AppExited;
            setup.ApplicationBase = pathToApp;

        }
        private void StartProcess()
        {
            var setup = CreateSetup();
            process = new Process();
            process.StartInfo = setup;
            process.EnableRaisingEvents = true;

            process.Exited += AppExited;

            process.Start();


            if (Environment.UserInteractive)
            {
                process.ErrorDataReceived +=
                    (sendingProcess, errorLine) => log.Error(string.Format("[{0}] {1}", process.ProcessName, errorLine.Data));
                process.OutputDataReceived +=
                    (sendingProcess, dataLine) => log.Info(string.Format("[{0}] {1}", process.ProcessName, dataLine.Data));

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
            }
        }
        void AppExited(object sender, EventArgs e)
        {
            log.Warn($"Application exited (Runmode:{runMode}");
        }


        private ProcessStartInfo CreateSetup()
        {
            var setup = new ProcessStartInfo();
            setup.FileName = GetExecutablePath();
            setup.WorkingDirectory = appCopy.ShadowPath;
            setup.UseShellExecute = false;
            setup.RedirectStandardOutput = true;
            setup.RedirectStandardError = true;

            //setup.RedirectStandardInput = true;

            return setup;
        }

        private string GetExecutablePath()
        {
            return Path.Combine(appCopy.ShadowPath, executable);
        }

        private readonly object appLock = new object();
        private void Stop()
        {
            lock (appLock)
                 switch (runMode)
                {
                    case RunMode.AppDomain:
                        AppDomain.Unload(domain);
                        domain = null;
                        break;
                    case RunMode.Process:
                        if (process != null)
                        {
                            process.Kill();
                            process.Close();
                            process = null;
                        }
                        break;
                    default:
                        throw new InvalidOperationException("Unknown runmode");
                }

        }


        public void Dispose()
        {
            watcher.Dispose();
            if (process != null)
            {
                Stop();
            }
        }

        public string Name { get { return name; } }
    }

}