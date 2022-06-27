using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using System.Management;
using System.IO;
using System.Linq;

namespace RansomwareSimulator.Helper
{
    public static class Recon
    {
        public static bool isElevated()
        {
            bool isElevated;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return isElevated;
        }

        public static List<string> ListShadowCopies()
        {
            string NamespacePath = "\\\\.\\ROOT\\cimv2";
            string ClassName = "Win32_ShadowCopy";
            List<string> listofshadows = new List<string>();
            //Create ManagementClass
            ManagementClass oClass = new ManagementClass(NamespacePath + ":" + ClassName);
            //Get all instances of the class and enumerate them
            try
            {
                foreach (ManagementObject oObject in oClass.GetInstances())
                {
                   listofshadows.Add( string.Concat(oObject.GetPropertyValue("DeviceObject"), " =>", oObject.GetPropertyValue("ID")));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not list shadow copies. Are you admin?. {e.Message}");
            }
            return listofshadows;
        }
        public static List<string> TraverseTree(string root, List<string> extensions,int maxfiles)
        {
            
            // Data structure to hold names of subfolders to be
            // examined for files.
            Stack<string> dirs = new Stack<string>(20);
            var files = new List<string>();
            if (!System.IO.Directory.Exists(root))
            {
                throw new ArgumentException();
            }
            dirs.Push(root);
            while (dirs.Count > 0)
            {
                string currentDir = dirs.Pop();
                string[] subDirs;
            
                try
                {
                    subDirs = Directory.GetDirectories(currentDir);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                catch (DirectoryNotFoundException)
                {
                    continue;
                }

              
                try
                {
                    if (files?.Count >= maxfiles)
                    {
                        break;
                    }
                    files.AddRange(Directory.GetFiles(currentDir, "*.*")
                             .Where(f => extensions.IndexOf(Path.GetExtension(f)) >= 0).ToArray());
                    files = files.Take(maxfiles).ToList();
                    Console.WriteLine("Looking @ {0} for {1}", currentDir, string.Join(" ;", extensions));
                }

                catch (UnauthorizedAccessException e)
                {

                    Console.WriteLine(e.Message);
                    continue;
                }

                catch (System.IO.DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                foreach (string str in subDirs)
                    dirs.Push(str);
            }
            return files;
        }
    }
}
