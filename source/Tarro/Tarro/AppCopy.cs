using System;
using System.IO;

namespace Tarro
{
    internal class AppCopy
    {
         private readonly string cachePath;
        private readonly string pathToApp;
        private readonly string executable;


        public AppCopy(string cachePath, string pathToApp, string executable)
        {
              this.cachePath = cachePath;
            this.pathToApp = pathToApp;
            this.executable = executable;
        }

        public string ShadowPath
        {
            get
            {
                var shadowPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cachePath, executable);
                return shadowPath;
            }
        }

        public void ShadowCopy()
        {
            CleanShadowDirectory();
            DirectoryCopy(pathToApp, ShadowPath, true);
        }

        private void CleanShadowDirectory()
        {
            var directory = new DirectoryInfo(ShadowPath);
            if (directory.Exists)
                directory.Delete(true);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}