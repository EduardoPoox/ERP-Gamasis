using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Gamasis.ProjectManagement.Controllers
{
    public class MailHelper
    {
        internal static int send(Mail mailtosend, List<string> path = null)
        {
            int res = 0;
            try
            {
                //string passwordFrom = System.Configuration.ConfigurationManager.AppSettings["passData"].ToString();
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(mailtosend.emailFrom, "Gamasis Project Management - " + DateTime.Now.Year, Encoding.UTF8);
                foreach (string item in mailtosend.emaillist)
                {
                    mail.To.Add(item);
                }
                if (path != null)
                {
                    foreach (string item in path)
                    {
                        mail.Attachments.Add(new Attachment(item));
                    }
                }
                if (mailtosend.attaches.Count != 0)
                {
                    foreach (objAttachment item in mailtosend.attaches)
                    {
                        mail.Attachments.Add(new Attachment(new MemoryStream(item.stream), item.name, item.mimeType));
                    }
                }
                SmtpClient SmtpServer = new SmtpClient(mailtosend.server.server, mailtosend.server.port);
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential(mailtosend.emailFrom, mailtosend.passwordFrom, "outlook.es");
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.EnableSsl = true;
                if (mailtosend.av != null)
                    mail.AlternateViews.Add(mailtosend.av);
                else
                {
                    mail.IsBodyHtml = true;
                    mail.Body = mailtosend.body;
                }
                mail.Subject = mailtosend.subject;
                try
                {
                    SmtpServer.Send(mail);
                    res = 1;
                }
                catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al enviar el email : {0} ", ex.Message)); }
            }
            catch (Exception ex) { res = 0; }
            return res;
        }
        public class objAttachment
        {
            public string name { set; get; }
            public string mimeType { set; get; }
            public byte[] stream { set; get; }
        }
        public class objServer
        {
            public objServer()
            {
                server = getAppSettingValue("serverData");
                port = int.Parse(getAppSettingValue("portData"));
            }

            public objServer(string s, int p)
            {
                server = s;
                port = p;
            }

            public static string getAppSettingValue(string key)
            {
                return System.Configuration.ConfigurationManager.AppSettings[key].ToString();
            }
            public string server { set; get; }
            public int port { set; get; }
        }
        public class Mail
        {
            public static string getAppSettingValue(string key)
            {
                return System.Configuration.ConfigurationManager.AppSettings[key].ToString();
            }
            public Mail(string from, string pass) { attaches = new List<objAttachment>(); this.emailFrom = from; passwordFrom = pass; emaillist = new List<string>(); server = new objServer(); }
            public Mail() { attaches = new List<objAttachment>(); emailFrom = getAppSettingValue("emailData"); passwordFrom = getAppSettingValue("passData"); emaillist = new List<string>(); server = new objServer(); }

            public string body { set; get; }
            public string subject { set; get; }
            public string emailFrom { set; get; }
            public string passwordFrom { set; get; }
            public List<objAttachment> attaches { set; get; }
            public List<string> emaillist { set; get; }
            public AlternateView av { set; get; }
            public objServer server { set; get; }
        }
    }
}