using Gamasis.ProjectManagement.Models.Objects;
using Gamasis.ProjectManagement.Models.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Gamasis.ProjectManagement.Controllers
{
    public class IncidentController : Controller
    {
        // GET: Incident
        [Authenticate(module = "Incident")]
        public ActionResult Index()
        {
            ViewBag.Incidents = PMIncident.get(SessionHelper.Account());
            ViewBag.Ubications = PMSetting.Get(SessionHelper.Account().data.comesfrom, 2);
            ViewBag.UserLogged = SessionHelper.Account();
            return View();
        }
        public JsonResult Get()
        {
            int upid = int.Parse(Request.QueryString.Get("upid"));
            return Json(PMIncident.getone(upid), "application/json", JsonRequestBehavior.AllowGet);
        }
        [Authenticate(module = "Incident")]
        public ActionResult Info()
        {
            int id = int.Parse(Request.QueryString.Get("i"));
            ViewBag.Inc = PMIncident.getone(id);
            ViewBag.Distlist = PMDistribution.Get();
            ViewBag.UserLogged = SessionHelper.Account();
            return View();
        }
        [Authenticate]
        public int Add(HttpPostedFileBase[] files)
        {
            #region attr
            int res = 0;
            CultureInfo provider = CultureInfo.InvariantCulture;
            Incident incident = new Incident();
            #endregion
            #region recollecteData
            incident.name = Request.Form.Get("name");
            incident.description = Request.Form.Get("description");
            try { incident.description = incident.description.Replace("'", "\\'"); } catch { }
            try { incident.description = incident.description.Replace("’", "\\'"); } catch { }
            try { incident.description = incident.description.Replace("´", "\\'"); } catch { }


            incident.priority = int.Parse(Request.Form.Get("priority"));
            incident.status = 1;
            incident.idaccount = SessionHelper.Account().id;
            incident.ubication = Request.Form.Get("ubication");
            try { incident.date = DateTime.Parse(Request.Form.Get("createdAt"), provider).ToString("yyyy-MM-dd HH:mm"); } catch { }
            if (files != null)
            {
                foreach (HttpPostedFileBase f in files)
                {
                    if (f != null)
                    {
                        var file = new Models.Objects.File()
                        {
                            name = f.FileName,
                            extension = f.FileName.Split('.').Last(),
                            mime = f.ContentType,
                            type = 1 //Incident
                        };
                        using (Stream inputStream = f.InputStream)
                        {
                            MemoryStream memoryStream = inputStream as MemoryStream;
                            if (memoryStream == null)
                            {
                                memoryStream = new MemoryStream();
                                inputStream.CopyTo(memoryStream);
                            }
                            file.data = memoryStream.ToArray();
                        }
                        incident.files.Add(file);
                    }
                }
            }
            #endregion
            #region operation
            res = PMIncident.create(incident);
            #endregion
            if (res != 0)
            {
                #region log
                Log nlog = new Log();
                nlog.ip = SessionHelper.Account().ip;
                nlog.accountid = SessionHelper.Account().id;
                nlog.date = incident.date;
                nlog.idrel = res;
                nlog.type = 3;
                nlog.subtype = 1;
                HttpRequest req = System.Web.HttpContext.Current.Request;
                nlog.navigator = req.Browser.Browser;
                try { PMUtils.insertlog(nlog); } catch { }
                #endregion
                #region Notification //To all??

                var account = PMIncident.getone(res);
                MailHelper.Mail mail = new Controllers.MailHelper.Mail();
                StringWriter writer = new StringWriter();
                HtmlTextWriter html = new HtmlTextWriter(writer);
                mail.body = "";
                mail.subject = "Notificación, Gamasis Project Management - " + account.name;
                html.RenderBeginTag(HtmlTextWriterTag.P);
                html.AddAttribute(HtmlTextWriterAttribute.Src, "http://gamasis.com.mx/Content/Images/GamasisGrayBlue.png");
                html.AddAttribute(HtmlTextWriterAttribute.Width, "340");
                html.AddAttribute(HtmlTextWriterAttribute.Height, "100");
                html.RenderBeginTag(HtmlTextWriterTag.Img);
                html.RenderEndTag();
                html.RenderEndTag();
                html.RenderBeginTag(HtmlTextWriterTag.H1);
                html.WriteEncodedText("Reporte de nueva incidencia - " + account.name);
                html.RenderEndTag();
                html.WriteEncodedText(String.Format("Estimado usuario de GPM"));
                html.WriteBreak();
                html.RenderBeginTag(HtmlTextWriterTag.P);
                html.WriteEncodedText("El motivo de este correo, es para notificar que se ha reportado una nueva incidencia por el usuario: ");
                html.RenderBeginTag(HtmlTextWriterTag.Strong);
                html.WriteEncodedText(account.accountName + "  con fecha de registro: " + DateTime.Parse(account.date).ToString("dd/MM/yyyy HH:mm") + " ");
                html.RenderEndTag();
                html.WriteEncodedText(" Este será visible en el módulo de incidencias, puedes tomar la incidencia desde ahí mismo para tener control sobre ella\n Sin más por el momento, ");
                html.RenderBeginTag(HtmlTextWriterTag.Strong);
                html.WriteEncodedText("gracias.");
                html.RenderEndTag();
                html.RenderEndTag();
                html.RenderBeginTag(HtmlTextWriterTag.P);
                html.WriteEncodedText("     \n No responda a este email.");
                html.RenderEndTag();
                html.Flush();
                mail.body = writer.ToString();
                #region mailAttributes
                List<string> mails = PMUtils.getMails(SessionHelper.Account().data.comesfrom, 1); // 1 = PROGRAMADORES ASIGNADOS A LA COMPANIA
                foreach (string i in mails)
                {
                    mail.emaillist.Add(i);
                }
                #endregion
                #region sendNotificacionMail
                res = MailHelper.send(mail);
                #endregion
                #endregion
            }
            return res;
        }
        [Authenticate]
        public int Save(HttpPostedFileBase[] files)
        {
            #region attr
            int res = 0;
            CultureInfo provider = CultureInfo.InvariantCulture;
            Incident incident = new Incident();
            #endregion
            #region recollecteData
            try { incident.id = int.Parse(Request.Form.Get("id")); } catch { }
            try { incident.name = Request.Form.Get("name"); } catch { }
            try { incident.description = Request.Form.Get("description"); } catch { }
            try { incident.priority = int.Parse(Request.Form.Get("priority")); } catch { }
            try { incident.status = int.Parse(Request.Form.Get("status")); } catch { }
            try { incident.progress = int.Parse(Request.Form.Get("progress")); } catch { }
            try { incident.idaccount = SessionHelper.Account().id; } catch { }
            try { incident.ubication = Request.Form.Get("ubication"); } catch { }
            try { incident.date = DateTime.Parse(Request.Form.Get("updatedAt"), provider).ToString("yyyy-MM-dd HH:mm"); } catch { }
            if (incident.status == 4)
                incident.progress = 100;
            if (files != null)
            {
                foreach (HttpPostedFileBase f in files)
                {
                    var file = new Models.Objects.File()
                    {
                        name = f.FileName,
                        extension = f.FileName.Split('.').Last(),
                        mime = f.ContentType,
                        type = 1 //Incident
                    };
                    using (Stream inputStream = f.InputStream)
                    {
                        MemoryStream memoryStream = inputStream as MemoryStream;
                        if (memoryStream == null)
                        {
                            memoryStream = new MemoryStream();
                            inputStream.CopyTo(memoryStream);
                        }
                        file.data = memoryStream.ToArray();
                    }
                    incident.files.Add(file);
                }
            }
            #endregion
            #region operation
            res = PMIncident.update(incident);
            #endregion
            #region log
            Log nlog = new Log();
            nlog.ip = SessionHelper.Account().ip;
            nlog.accountid = SessionHelper.Account().id;
            nlog.idrel = incident.id;
            nlog.date = incident.date;
            nlog.type = 3;
            nlog.subtype = 2;
            nlog.val = "Status - " + incident.status.ToString();
            HttpRequest req = System.Web.HttpContext.Current.Request;
            nlog.navigator = req.Browser.Browser;
            try { PMUtils.insertlog(nlog); } catch { }
            #endregion
            #region notification
            var account = PMIncident.getone(incident.id);
            MailHelper.Mail mail = new Controllers.MailHelper.Mail();
            StringWriter writer = new StringWriter();
            HtmlTextWriter html = new HtmlTextWriter(writer);
            mail.body = "";
            mail.subject = "Notificación, Gamasis Project Management - " + account.name;
            html.RenderBeginTag(HtmlTextWriterTag.P);
            html.AddAttribute(HtmlTextWriterAttribute.Src, "http://gamasis.com.mx/Content/Images/GamasisGrayBlue.png");
            html.AddAttribute(HtmlTextWriterAttribute.Width, "340");
            html.AddAttribute(HtmlTextWriterAttribute.Height, "100");
            html.RenderBeginTag(HtmlTextWriterTag.Img);
            html.RenderEndTag();
            html.RenderEndTag();
            html.RenderBeginTag(HtmlTextWriterTag.H1);
            html.WriteEncodedText("Actualización reciente - " + account.name);
            html.RenderEndTag();

            html.WriteEncodedText(String.Format("Estimado {0},", account.accountName));
            html.WriteBreak();
            html.RenderBeginTag(HtmlTextWriterTag.P);
            html.WriteEncodedText("El motivo de este correo, es para notificar que la incidencia reportada ");
            html.RenderBeginTag(HtmlTextWriterTag.Strong);
            html.WriteEncodedText(account.name + "  con fecha: " + account.date + " ");
            html.RenderEndTag();
            html.WriteEncodedText(" ha sido modificado en la plataforma\n El Status actual es: ");
            html.RenderBeginTag(HtmlTextWriterTag.Strong);
            html.WriteEncodedText((account.status == 1 ? "Recibido" : (account.status == 2 ? "Diagnóstico" : (account.status == 3 ? "Desarrollo" : (account.status == 4 ? "Retroalimentación" : (account.status == 5 ? "Concluido" : "Cancelado"))))));
            html.RenderEndTag();
            html.RenderEndTag();
            html.RenderBeginTag(HtmlTextWriterTag.P);
            html.WriteEncodedText("El cual lleva un progreso de avance del: ");
            html.RenderBeginTag(HtmlTextWriterTag.Strong);
            html.WriteEncodedText(account.progress.ToString() + " %");
            html.RenderEndTag();
            html.RenderEndTag();
            html.RenderBeginTag(HtmlTextWriterTag.P);
            html.WriteEncodedText("     \n Por su atención, gracias y no responda a este email.");
            html.RenderEndTag();
            html.Flush();
            mail.body = writer.ToString();
            #region mailAttributes
            mail.emaillist.Add(account.accountMail);
            #endregion
            #region sendNotificacionMail
            res = MailHelper.send(mail);
            #endregion


            #endregion
            return res;
        }
        [Authenticate]
        public int Assing()
        {
            #region Attr
            int res = 0;
            int id = 0;
            string date = "";
            CultureInfo provider = CultureInfo.InvariantCulture;
            #endregion
            #region Attr
            try { id = int.Parse(Request.QueryString.Get("idinc")); } catch { }
            try { date = DateTime.Parse(Request.QueryString.Get("date"), provider).ToString("yyyy-MM-dd HH:mm"); } catch { }
            #endregion
            #region operation
            if (id != 0)
                res = PMProgrammer.setprogrammer(id, SessionHelper.Account().id);
            #endregion
            if (res != 0)
            {
                #region log
                Log nlog = new Log();
                nlog.ip = SessionHelper.Account().ip;
                nlog.accountid = SessionHelper.Account().id;
                nlog.date = date;
                nlog.idrel = id;
                nlog.type = 3;
                nlog.subtype = 3;
                nlog.val = "Programmer";
                HttpRequest req = System.Web.HttpContext.Current.Request;
                nlog.navigator = req.Browser.Browser;
                try { PMUtils.insertlog(nlog); } catch { }
                #endregion
                #region notification
                var account = PMIncident.getone(id);
                MailHelper.Mail mail = new Controllers.MailHelper.Mail();
                StringWriter writer = new StringWriter();
                HtmlTextWriter html = new HtmlTextWriter(writer);
                mail.body = "";
                mail.subject = "Notificación, Gamasis Project Management - " + account.name;
                html.RenderBeginTag(HtmlTextWriterTag.P);
                html.AddAttribute(HtmlTextWriterAttribute.Src, "http://gamasis.com.mx/Content/Images/GamasisGrayBlue.png");
                html.AddAttribute(HtmlTextWriterAttribute.Width, "340");
                html.AddAttribute(HtmlTextWriterAttribute.Height, "100");
                html.RenderBeginTag(HtmlTextWriterTag.Img);
                html.RenderEndTag();
                html.RenderEndTag();
                html.RenderBeginTag(HtmlTextWriterTag.H1);
                html.WriteEncodedText("Actualización reciente - " + account.name);
                html.RenderEndTag();
                html.WriteEncodedText(String.Format("Estimado {0},", account.accountName));
                html.WriteBreak();
                html.RenderBeginTag(HtmlTextWriterTag.P);
                html.WriteEncodedText("El motivo de este correo, es para notificar que en la incidencia que reportaste ");
                html.RenderBeginTag(HtmlTextWriterTag.Strong);
                html.WriteEncodedText(account.name + "  con fecha de registro: " + DateTime.Parse(account.date).ToString("dd/MM/yyyy HH:mm") + " ");
                html.RenderEndTag();
                html.WriteEncodedText(string.Format(" ha sido tomada por el desarrollador {0} en la plataforma\n Por lo pronto, deberá a esperar a que las acciones se den, y los estados de la incidencia se reportarán conformen se vaya avanzando. \n ", (SessionHelper.Account().data.name + " " + SessionHelper.Account().data.lastname)));
                html.RenderBeginTag(HtmlTextWriterTag.Strong);
                html.WriteEncodedText("Gracias.");
                html.RenderEndTag();
                html.RenderEndTag();
                html.RenderBeginTag(HtmlTextWriterTag.P);
                html.WriteEncodedText("     \n No responda a este email.");
                html.RenderEndTag();
                html.Flush();
                mail.body = writer.ToString();
                #region mailAttributes
                mail.emaillist.Add(account.accountMail);
                #endregion
                #region sendNotificacionMail
                res = MailHelper.send(mail);
                #endregion
                #endregion
            }
            return res;
        }
        [Authenticate]
        public int End()
        {
            #region Attr
            int res = 0;
            int idinc = 0;
            string date = "";
            CultureInfo provider = CultureInfo.InvariantCulture;
            #endregion
            #region Attr
            try { idinc = int.Parse(Request.QueryString.Get("idrel")); } catch { }
            try { date = DateTime.Parse(Request.QueryString.Get("date"), provider).ToString("yyyy-MM-dd HH:mm"); } catch { }
            #endregion
            if (idinc != 0)
                res = PMIncident.setConcluded(SessionHelper.Account().id, idinc, date);
            if (res != 0)
            {
                #region log
                Log nlog = new Log();
                nlog.ip = SessionHelper.Account().ip;
                nlog.accountid = SessionHelper.Account().id;
                nlog.date = date;
                nlog.idrel = idinc;
                nlog.type = 3;
                nlog.subtype = 2;
                nlog.val = "Status - 5";
                HttpRequest req = System.Web.HttpContext.Current.Request;
                nlog.navigator = req.Browser.Browser;
                try { PMUtils.insertlog(nlog); } catch { }
                #endregion
                #region notification
                var account = PMIncident.getone(idinc);
                MailHelper.Mail mail = new Controllers.MailHelper.Mail();
                StringWriter writer = new StringWriter();
                HtmlTextWriter html = new HtmlTextWriter(writer);
                mail.body = "";
                mail.subject = "Notificación, Gamasis Project Management - " + account.name;
                html.RenderBeginTag(HtmlTextWriterTag.P);
                html.AddAttribute(HtmlTextWriterAttribute.Src, "http://gamasis.com.mx/Content/Images/GamasisGrayBlue.png");
                html.AddAttribute(HtmlTextWriterAttribute.Width, "340");
                html.AddAttribute(HtmlTextWriterAttribute.Height, "100");
                html.RenderBeginTag(HtmlTextWriterTag.Img);
                html.RenderEndTag();
                html.RenderEndTag();
                html.RenderBeginTag(HtmlTextWriterTag.H1);
                html.WriteEncodedText("Actualización reciente - " + account.name);
                html.RenderEndTag();
                html.WriteEncodedText(String.Format("Estimado {0},", account.accountName));
                html.WriteBreak();
                html.RenderBeginTag(HtmlTextWriterTag.P);
                html.WriteEncodedText("El motivo de este correo, es para notificar que la incidencia solicitado ");
                html.RenderBeginTag(HtmlTextWriterTag.Strong);
                html.WriteEncodedText(account.name + "  con fecha de registro: " + DateTime.Parse(account.date).ToString("dd/MM/yyyy HH:mm") + " ");
                html.RenderEndTag();
                html.WriteEncodedText(" ha sido modificado en la plataforma\n Ahora, el cliente ha notificado de que la incidencia reportada ya está lista y ");
                html.RenderBeginTag(HtmlTextWriterTag.Strong);
                html.WriteEncodedText("CONCLUÍDA.");
                html.RenderEndTag();
                html.RenderEndTag();
                html.RenderBeginTag(HtmlTextWriterTag.P);
                html.WriteEncodedText("     \n Por su atención, gracias y no responda a este email.");
                html.RenderEndTag();
                html.Flush();
                mail.body = writer.ToString();
                #region mailAttributes
                List<string> mails = PMUtils.getMails(SessionHelper.Account().data.comesfrom, 1); // 1 = PROGRAMADORES ASIGNADOS A LA COMPANIA
                foreach (string i in mails)
                {
                    mail.emaillist.Add(i);
                }
                #endregion
                #region sendNotificacionMail
                res = MailHelper.send(mail);
                #endregion
                #endregion
            }
            return res;
        }
        public int AddFiles(HttpPostedFileBase[] files)
        {
            #region attr
            int res = 0;
            CultureInfo provider = CultureInfo.InvariantCulture;
            List<Models.Objects.File> forAdd = new List<Models.Objects.File>();
            #endregion
            #region recollectedData
            var idrel = int.Parse(Request.Form.Get("idrel"));
            if (files != null)
            {
                foreach (HttpPostedFileBase f in files)
                {
                    if (f != null)
                    {
                        var file = new Models.Objects.File()
                        {
                            name = f.FileName,
                            extension = f.FileName.Split('.').Last(),
                            mime = f.ContentType,
                            type = 1, //Incident
                            idrel = idrel
                        };
                        using (Stream inputStream = f.InputStream)
                        {
                            MemoryStream memoryStream = inputStream as MemoryStream;
                            if (memoryStream == null)
                            {
                                memoryStream = new MemoryStream();
                                inputStream.CopyTo(memoryStream);
                            }
                            file.data = memoryStream.ToArray();
                        }
                        forAdd.Add(file);
                    }
                }
            }
            #endregion
            #region operation
            res = PMFile.addFiles(forAdd);
            #endregion
            return res;
        }
        public int Remove()
        {
            #region attr
            int res = 0;
            CultureInfo provider = CultureInfo.InvariantCulture;
            var idrel = int.Parse(Request.QueryString.Get("idrel"));
            var date = "";
            try { date = DateTime.Parse(Request.QueryString.Get("date"), provider).ToString("yyyy-MM-dd HH:mm"); } catch { }
            var incident = PMIncident.getone(idrel);
            #endregion
            #region operation
            res = PMIncident.remove(idrel);
            #endregion
            if (res != 0)
            {
                #region log
                Log nlog = new Log();
                nlog.comments = incident.name;
                nlog.ip = SessionHelper.Account().ip;
                nlog.accountid = SessionHelper.Account().id;
                nlog.date = date;
                nlog.idrel = idrel;
                nlog.type = 3;
                nlog.subtype = 4;
                HttpRequest reqq = System.Web.HttpContext.Current.Request;
                nlog.navigator = reqq.Browser.Browser;
                try { PMUtils.insertlog(nlog); } catch { }
                #endregion
            }
            return res;
        }
    }
}