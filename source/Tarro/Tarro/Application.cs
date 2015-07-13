using System;
using System.Diagnostics;
using System.IO;
using Tarro.Logging;

namespace Tarro
{
    internal class Application : IDisposable
    {
        private readonly ILog log = LogFactory.GetLogger<Application>();
        private readonly string name;
        private readonly string pathToApp;
        private readonly string executable;
        private readonly AppWatcher watcher;
        private readonly AppCopy appCopy;
        private Process process;

        private readonly string cachePath = "appCache";
        public Application(string name, string pathToApp, string executable)
        {
            this.name = name;
            this.pathToApp = pathToApp;
            this.executable = executable;
            appCopy =  new AppCopy(cachePath,pathToApp,executable);
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

                lock (processLock)
                {
                    if(process!=null) //Should be null if stopped
                        return;
                    log.Info("Starting application ({0})", name);
                    appCopy.ShadowCopy();
                    var setup = CreateSetup();
                    process = new Process();
                    process.StartInfo = setup;
                    process.EnableRaisingEvents = true;
                    process.Exited += process_Exited;

                    process.Start();


                    if (Environment.UserInteractive)
                    {
                        process.ErrorDataReceived += (sendingProcess, errorLine) => log.Error(string.Format("[{0}] {1}", process.ProcessName, errorLine.Data));
                        process.OutputDataReceived += (sendingProcess, dataLine) => log.Info(string.Format("[{0}] {1}", process.ProcessName, dataLine.Data));

                        process.BeginErrorReadLine();
                        process.BeginOutputReadLine();


                    } 
                    log.Info("Application started ({0})", name);
                }

            }
            catch (Exception ex)
            {
                log.Error("Unable to start application ({0})", ex, name);
            }
        }

        void process_Exited(object sender, EventArgs e)
        {
            log.Warn("Process exit");
        }
     

        private ProcessStartInfo CreateSetup()
        {
            var setup = new ProcessStartInfo();
            setup.FileName = Path.Combine(appCopy.ShadowPath, executable);
            setup.WorkingDirectory = appCopy.ShadowPath;
            setup.UseShellExecute = false;
            setup.RedirectStandardOutput = true;
            setup.RedirectStandardError = true;
            //setup.RedirectStandardInput = true;

            return setup;
        }

        private readonly object processLock = new object();
        private void Stop()
        {
            lock (processLock)
                if (process != null)
                {
                    process.Kill();
                    process.Close();
                    process = null;
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