using System;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using System.Threading;
using Tarro.Logging;

namespace Tarro
{
    internal abstract class AppRuntime
    {
        protected readonly string name;
        protected readonly string pathToApp;
        protected readonly string executable;
        protected readonly string shadowPath;
        protected readonly ILog log = LogFactory.GetLogger<AppRuntime>();
        protected AppRuntime(string name, string pathToApp, string executable, string shadowPath)
        {
            this.name = name;
            this.pathToApp = pathToApp;
            this.executable = executable;
            this.shadowPath = shadowPath;
        }

        public abstract void Start();

        public abstract void Stop();
        public abstract bool IsActive();


        protected string GetExecutablePath()
        {
            return Path.Combine(shadowPath, executable);
        }
        protected void AppExited(object sender, EventArgs e)
        {
            log.Warn($"Application exited");
        }
    }

    internal class AppDomainRuntime : AppRuntime
    {
        private AppDomain domain;
        private Thread thread;

        public override void Stop()
        {
            try
            {
                if (domain != null)
                {
                    if (!thread.Join(25))
                        thread.Abort();
                    AppDomain.Unload(domain);
                    domain = null;
                }
            }
            catch (AppDomainUnloadedException adue)
            {

                log.Warn("Appdomain was previously unloaded", adue);
            }

        }

        public override bool IsActive()
        {
            return domain != null;
        }

        public override void Start()
        {
            var setup = AppDomainSetup();

            try
            {
                domain = AppDomain.CreateDomain(executable, new Evidence(), setup);
                thread = new Thread(() => domain.ExecuteAssembly(GetExecutablePath()));
                thread.Start();

            }
            catch (Exception)
            {
                AppDomain.Unload(domain);
                domain = null;
                thread = null;
                throw;
            }
        }
        private AppDomainSetup AppDomainSetup()
        {
            var setup = new AppDomainSetup();
            setup.ApplicationBase = pathToApp;

            setup.ApplicationBase = shadowPath;
            setup.PrivateBinPath = shadowPath;
            var potentialConfigFile = executable + ".config";
            if (File.Exists(Path.Combine(pathToApp, potentialConfigFile)))
                setup.ConfigurationFile = potentialConfigFile;
            return setup;
        }

        public AppDomainRuntime(string name, string pathToApp, string executable, string shadowPath) : base(name, pathToApp, executable, shadowPath)
        {
        }
    }

    internal class ProcessRuntime : AppRuntime
    {
        private Process process;


        public override void Stop()
        {
            if (process != null)
            {
                process.StandardInput.Write("\x3");
                process.WaitForExit(25);
                if (!process.HasExited)
                    process.Kill();
                process.Close();
                process = null;
            }
        }

        public override bool IsActive()
        {
            return process != null;
        }

        public override void Start()
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
                    (sendingProcess, errorLine) => log.Error($"[{process.ProcessName}] {errorLine.Data}");
                process.OutputDataReceived +=
                    (sendingProcess, dataLine) => log.Info($"[{process.ProcessName}] {dataLine.Data}");

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
            }
        }
        private ProcessStartInfo CreateSetup()
        {
            var setup = new ProcessStartInfo();
            setup.FileName = GetExecutablePath();
            setup.WorkingDirectory = shadowPath;
            setup.UseShellExecute = false;
            setup.RedirectStandardOutput = true;
            setup.RedirectStandardError = true;
            setup.RedirectStandardInput = true;

            return setup;
        }

        public ProcessRuntime(string name, string pathToApp, string executable, string shadowPath) : base(name, pathToApp, executable, shadowPath)
        {
        }
    }
}