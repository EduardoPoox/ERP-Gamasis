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
    public class RequirementController : Controller
    {
        // GET: Requirement
        [Authenticate(module = "Requirement")]
        public ActionResult Index()
        {
            ViewBag.UserLogged = SessionHelper.Account();
            ViewBag.Ubications = PMSetting.Get(SessionHelper.Account().data.comesfrom, 2);
            ViewBag.Requirements = PMRequirement.get(SessionHelper.Account());
            return View();
        }
        [Authenticate]
        public ActionResult Create()
        {
            if (Session["trequirement"] == null)
                return RedirectToAction("Index", "Requirement");
            ViewBag.UserLogged = SessionHelper.Account();
            ViewBag.Requirement = (Requirement)Session["trequirement"];
            return View();
        }
        [HttpPost]
        [Authenticate]
        [ValidateInput(false)]
        public int Add(HttpPostedFileBase[] files)
        {
            #region attr
            int res = 0;
            CultureInfo provider = CultureInfo.InvariantCulture;
            Requirement req = new Requirement();
            Requirement temp = (Requirement)Session["trequirement"];
            #endregion
            #region recollecteData
            req.name = temp.name;
            req.description = Request.Form.Get("description");
            try { req.description = req.description.Replace("'", "\\'"); } catch { }
            try { req.description = req.description.Replace("’", "\\'"); } catch { }
            try { req.description = req.description.Replace("´", "\\'"); } catch { }

            try { req.template = Request.Form.Get("templatetext"); } catch (Exception ex) { }
            try { req.priority = int.Parse(Request.Form.Get("priority")); } catch (Exception ex) { }

            req.status = 1;
            req.idaccount = SessionHelper.Account().id;
            req.ubication = temp.ubication;
            try { req.date = DateTime.Parse(Request.Form.Get("createdAt"), provider).ToString("yyyy-MM-dd HH:mm:ss"); } catch { }
            req.type = temp.type;
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
                            type = 2 //Requirement
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
                        req.files.Add(file);
                    }
                }
            }
            #endregion
            #region operation
            res = PMRequirement.create(req);
            #endregion
            if (res > 0)
            {
                #region log
                Log nlog = new Log();
                nlog.ip = SessionHelper.Account().ip;
                nlog.accountid = SessionHelper.Account().id;
                nlog.date = req.date;
                nlog.idrel = res;
                nlog.type = 4;
                nlog.subtype = 1;
                HttpRequest reqq = System.Web.HttpContext.Current.Request;
                nlog.navigator = reqq.Browser.Browser;
                try { PMUtils.insertlog(nlog); } catch { }
                #endregion
                #region Notification //To all??
                var account = PMRequirement.getone(res);
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
                html.WriteEncodedText("Nuevo requerimiento solicitado - " + account.name);
                html.RenderEndTag();
                html.WriteEncodedText(String.Format("Estimado usuario de GPM"));
                html.WriteBreak();
                html.RenderBeginTag(HtmlTextWriterTag.P);
                html.WriteEncodedText("El motivo de este correo, es para notificar que se ha solicitado un nuevo requerimiento por el usuario: ");
                html.RenderBeginTag(HtmlTextWriterTag.Strong);
                html.WriteEncodedText(account.name + "  con fecha de registro: " + DateTime.Parse(account.date).ToString("dd/MM/yyyy HH:mm") + " ");
                html.RenderEndTag();
                html.WriteEncodedText(" Este será visible en el módulo de requerimientos, puedes tomar la solicitud desde ahí mismo para tener control sobre ella\n Sin más por el momento, ");
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
        public int Temp()
        {
            #region attr
            int res = 0;
            Requirement temp = new Requirement();
            #endregion
            #region recollecteData
            try { temp.name = Request.Form.Get("name"); } catch (Exception ex) { }
            try { temp.description = Request.Form.Get("description"); } catch (Exception ex) { }
            try { temp.priority = int.Parse(Request.Form.Get("priority")); } catch (Exception ex) { }
            try { temp.status = 1; } catch (Exception ex) { }
            try { temp.idaccount = SessionHelper.Account().id; } catch (Exception ex) { }
            try { temp.ubication = Request.Form.Get("ubication"); } catch (Exception ex) { }
            try { temp.type = int.Parse(Request.Form.Get("type")); } catch (Exception ex) { }
            try { temp.columns = int.Parse(Request.Form.Get("ncolumns")); } catch (Exception ex) { }
            try { temp.showhelper = (Request.Form.Get("showhelper") == "on" ? true : false); } catch (Exception ex) { temp.showhelper = false; }
            try { Session.Remove("trequirement"); } catch { }
            try
            {
                Session.Add("trequirement", temp);
                res = 1;
            }
            catch (Exception ex) { }
            #endregion
            return res;
        }
        [Authenticate(module = "Requirement")]
        public ActionResult Info()
        {
            int id = int.Parse(Request.QueryString.Get("i"));
            ViewBag.Req = PMRequirement.getone(id);
            ViewBag.Distlist = PMDistribution.Get();
            ViewBag.UserLogged = SessionHelper.Account();
            return View();
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
            try { date = DateTime.Parse(Request.QueryString.Get("date"), provider).ToString("yyyy-MM-dd HH:mm:ss"); } catch { }
            #endregion
            #region operation
            if (id != 0)
                res = PMProgrammer.setprogrammer(id, SessionHelper.Account().id, "req");
            #endregion
            if (res != 0)
            {
                #region log
                Log nlog = new Log();
                nlog.ip = SessionHelper.Account().ip;
                nlog.accountid = SessionHelper.Account().id;
                nlog.date = date;
                nlog.idrel = id;
                nlog.type = 4;
                nlog.subtype = 3;
                nlog.val = "Programmer";
                HttpRequest req = System.Web.HttpContext.Current.Request;
                nlog.navigator = req.Browser.Browser;
                try { PMUtils.insertlog(nlog); } catch { }
                #endregion

                #region notification
                var account = PMRequirement.getone(id);
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
                html.WriteEncodedText("El motivo de este correo, es para notificar que en el requerimiento que solicitaste ");
                html.RenderBeginTag(HtmlTextWriterTag.Strong);
                html.WriteEncodedText(account.name + "  con fecha de registro: " + DateTime.Parse(account.date).ToString("dd/MM/yyyy HH:mm:ss") + " ");
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
        public int Save(HttpPostedFileBase[] files)
        {
            #region attr
            int res = 0;
            CultureInfo provider = CultureInfo.InvariantCulture;
            Requirement req = new Requirement();
            #endregion
            #region recollecteData
            try { req.id = int.Parse(Request.Form.Get("id")); } catch { }
            try { req.name = Request.Form.Get("name"); } catch { }
            try { req.description = Request.Form.Get("description"); } catch { }
            try { req.priority = int.Parse(Request.Form.Get("priority")); } catch { }


            try { req.status = int.Parse(Request.Form.Get("status")); } catch { }


            try { req.progress = int.Parse(Request.Form.Get("progress")); } catch { }
            //try { req.idaccount = SessionHelper.Account().id; } catch { }
            try { req.ubication = Request.Form.Get("ubication"); } catch { }
            try { req.template = Request.Form.Get("template"); } catch { }
            try { req.date = DateTime.Parse(Request.Form.Get("updatedAt"), provider).ToString("yyyy-MM-dd HH:mm:ss"); } catch { }

            if (req.status > 30) // is a subtype
            {
                req.statussubtype = int.Parse(req.status.ToString().Substring(1, 1));
                req.status = 3;
                if (req.statussubtype == 4)
                    req.progress = 100;
            }
            if (req.status == 4)
                req.progress = 100;

            if (files != null)
            {
                foreach (HttpPostedFileBase f in files)
                {
                    var file = new Models.Objects.File()
                    {
                        name = f.FileName,
                        extension = f.FileName.Split('.').Last(),
                        mime = f.ContentType,
                        type = 2 //req
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
                    req.files.Add(file);
                }
            }
            #endregion
            #region operation
            res = PMRequirement.update(req);
            #endregion
            #region log
            Log nlog = new Log();
            nlog.ip = SessionHelper.Account().ip;
            nlog.accountid = SessionHelper.Account().id;
            nlog.idrel = req.id;
            nlog.date = req.date;
            nlog.type = 4;
            nlog.subtype = 2;
            nlog.val = "Status - " + req.status.ToString();
            HttpRequest reqq = System.Web.HttpContext.Current.Request;
            nlog.navigator = reqq.Browser.Browser;
            try { PMUtils.insertlog(nlog); } catch { }
            #endregion
            #region notification
            if (res == 1)
            {
                var account = PMRequirement.getone(req.id);

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
                html.WriteEncodedText("El motivo de este correo, es para notificar que el requerimiento solicitado ");
                html.RenderBeginTag(HtmlTextWriterTag.Strong);
                html.WriteEncodedText(account.name + "  con fecha: " + account.date + " ");
                html.RenderEndTag();
                html.WriteEncodedText(" ha sido modificado en la plataforma\n El Status actual es: ");
                html.RenderBeginTag(HtmlTextWriterTag.Strong);
                html.WriteEncodedText((account.status == 1 ? "Espera de aprobación" : (account.status == 2 ? "Aprobado" : (account.status == 3 ? "Procesando" : (account.status == 4 ? "Revisión del cliente" : (account.status == 5 ? "Finalizado" : "Cancelado"))))));
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
            }
            #endregion
            return res;
        }
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
            #region operation
            if (idinc != 0)
                res = PMRequirement.setConcluded(SessionHelper.Account().id, idinc, date);
            #endregion
            if (res != 0)
            {
                #region log
                Log nlog = new Log();
                nlog.ip = SessionHelper.Account().ip;
                nlog.accountid = SessionHelper.Account().id;
                nlog.date = date;
                nlog.idrel = idinc;
                nlog.type = 4;
                nlog.subtype = 2;
                nlog.val = "Status - 5";
                HttpRequest req = System.Web.HttpContext.Current.Request;
                nlog.navigator = req.Browser.Browser;
                try { PMUtils.insertlog(nlog); } catch { }
                #endregion
                #region notification
                var account = PMRequirement.getone(idinc);
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
                html.WriteEncodedText("El motivo de este correo, es para notificar que el requerimiento solicitado ");
                html.RenderBeginTag(HtmlTextWriterTag.Strong);
                html.WriteEncodedText(account.name + "  con fecha de registro: " + DateTime.Parse(account.date).ToString("dd/MM/yyyy HH:mm") + " ");
                html.RenderEndTag();
                html.WriteEncodedText(" ha sido modificado en la plataforma\n Ahora, el cliente ha notificado de que el requerimiento ya está listo y ");
                html.RenderBeginTag(HtmlTextWriterTag.Strong);
                html.WriteEncodedText("CONCLUÍDO.");
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
        public int Next()
        {
            #region Attr
            int res = 0;
            int idinc = 0;
            string date = "";
            CultureInfo provider = CultureInfo.InvariantCulture;
            #endregion
            #region Attr
            try { idinc = int.Parse(Request.QueryString.Get("idrel")); } catch { }
            try { date = DateTime.Parse(Request.QueryString.Get("date"), provider).ToString("yyyy-MM-dd HH:mm:ss"); } catch { }
            #endregion
            #region operation
            if (idinc != 0)
                res = PMRequirement.setCoding(SessionHelper.Account().id, idinc);
            #endregion
            if (res != 0)
            {
                #region log
                Log nlog = new Log();
                nlog.ip = SessionHelper.Account().ip;
                nlog.accountid = SessionHelper.Account().id;
                nlog.date = date;
                nlog.idrel = idinc;
                nlog.type = 4;
                nlog.subtype = 2;
                nlog.val = "Status - 3 - sub - 2";
                HttpRequest req = System.Web.HttpContext.Current.Request;
                nlog.navigator = req.Browser.Browser;
                try { PMUtils.insertlog(nlog); } catch { }
                #endregion
                #region notification
                var account = PMRequirement.getone(idinc);
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
                html.WriteEncodedText("El motivo de este correo, es para notificar que el requerimiento solicitado ");
                html.RenderBeginTag(HtmlTextWriterTag.Strong);
                html.WriteEncodedText(account.name + "  con fecha de registro: " + DateTime.Parse(account.date).ToString("dd/MM/yyyy HH:mm:mm") + " ");
                html.RenderEndTag();
                html.WriteEncodedText(" ha sido modificado en la plataforma\n Ahora, el cliente ha notificado de que ya es posible pasar a la etapa de : ");
                html.RenderBeginTag(HtmlTextWriterTag.Strong);
                html.WriteEncodedText("CODIFICACIÓN");
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
                            type = 2, //Incident
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
            try { date = DateTime.Parse(Request.QueryString.Get("date"), provider).ToString("yyyy-MM-dd HH:mm:ss"); } catch { }
            var requirement = PMRequirement.getone(idrel);
            #endregion
            #region operation
            res = PMRequirement.remove(idrel);
            #endregion

            if (res != 0)
            {
                #region log
                Log nlog = new Log();
                nlog.comments = requirement.name;
                nlog.ip = SessionHelper.Account().ip;
                nlog.accountid = SessionHelper.Account().id;
                nlog.date = date;
                nlog.idrel = idrel;
                nlog.type = 4;
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