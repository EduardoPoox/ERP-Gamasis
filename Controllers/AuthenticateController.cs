using Gamasis.ProjectManagement.Models.Objects;
using Gamasis.ProjectManagement.Models.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Gamasis.ProjectManagement.Controllers
{
    public class AuthenticateController : Controller
    {
        // GET: Authenticate
        public ActionResult Index()
        {
            if (Session["GNT_SESSION"] != null)
                return RedirectToAction("Index", "Authenticate");

            return View();
        }
        public ActionResult Create()
        {
            if (Session["GNT_SESSION"] != null)
                return RedirectToAction("Index", "Authenticate");
            else if (Session["accountForConvert"] == null)
                return RedirectToAction("Index", "Authenticate");
            ViewBag.Info = (Account)Session["accountForConvert"];
            return View();
        }
        public int Start()
        {
            #region attr
            int res = 0;
            CultureInfo provider = CultureInfo.InvariantCulture;
            Account a = new Account();
            #endregion
            #region recollect data
            a.username = Request.Form.Get("username");
            a.password = Request.Form.Get("password");
            try { a.sessionStartedAt = DateTime.Parse(Request.Form.Get("startedAt"), provider).ToString("yyyy-MM-dd HH:mm:ss"); } catch { }
            #endregion
            #region operation
            WsUser.LoginUser gres = new WsUser.LoginUser();
            try { gres = new WsUser.Users().Login(a.username, a.password); } catch { }
            if (gres.Ok)
            {
                Alfas exists = PMLogin.Exists(gres.UserData.Id, gres.UserData.From);
                if (exists.iddata != 0) //It exists in gpmdb
                {
                    //Create sessionHelper
                    a.id = exists.iddata;
                    a.data.idfromproject = gres.UserData.Id;
                    a.data.comesfrom = gres.UserData.From;
                    a.data.email = gres.UserData.Email;
                    a.data.cellphone = gres.UserData.Phone;
                    a.data.name = string.Format("{0}", gres.UserData.Firstname);
                    a.data.lastname = string.Format("{0} {1}", gres.UserData.Lastname, gres.UserData.SecondLastname);
                    a.isactive = gres.UserData.Active;
                    a.data.rol = exists.rol;
                    a.data.rolstring = exists.rolstring;
                    a.data.activate = exists.activate;
                    a.assignations = PMAccount.GetAssignations(exists.iddata);
                    a.rol = exists.rol;
                    a.rolstring = exists.rolstring;
                    a.ip = Request.UserHostAddress;

                    #region Log
                    Log nlog = new Log();
                    nlog.ip = a.ip;
                    nlog.accountid = a.id;
                    nlog.date = a.sessionStartedAt;
                    nlog.type = 1;
                    nlog.val = "Login";
                    HttpRequest req = System.Web.HttpContext.Current.Request;
                    nlog.navigator = req.Browser.Browser;
                    #endregion
                    if (!exists.activate)
                    {
                        nlog.val = "MustActivate";
                        try { PMUtils.insertlog(nlog); } catch { }
                        Session["accountForActivate"] = a;
                        res = 3;
                        return res;
                    }
                    try { PMUtils.insertlog(nlog); } catch { }
                    Session.Add("GNT_SESSION", a);
                    res = 1;
                }
                else //we have to create a profile in gpmdb
                {
                    //if (string.IsNullOrEmpty(gres.UserData.Email) || string.IsNullOrEmpty(gres.UserData.Phone)) //We have to ask for those 
                    //{

                        a.id = exists.iddata;
                        a.data.idfromproject = gres.UserData.Id;
                        a.data.comesfrom = gres.UserData.From;
                        a.data.email = gres.UserData.Email;
                        a.data.cellphone = gres.UserData.Phone;
                        a.data.name = string.Format("{0}", gres.UserData.Firstname);
                        a.data.lastname = string.Format("{0} {1}", gres.UserData.Lastname, gres.UserData.SecondLastname);
                        a.isactive = true;
                        if (gres.UserData.From != "Gamasis" || gres.UserData.From != "BioMatcher")
                            a.data.rol = 2; //Customer
                        else
                            a.rol = 1;//ADMIN

                        #region Assignations
                        if (gres.UserData.From != "Gamasis" || gres.UserData.From != "BioMatcher")
                        {
                            a.assignations.Add(new Assignation()
                            {
                                type = 1,
                                value = "Incident"
                            });
                            a.assignations.Add(new Assignation()
                            {
                                type = 1,
                                value = "Requirement"
                            });
                        }
                        #endregion
                        a.ip = Request.UserHostAddress;
                        Session.Add("accountForConvert", a);
                        res = 2; //the user have to interact with its data
                    //}
                }
            }
            #endregion
            return res;
        }
        public int Convert()
        {
            #region Attr
            int res = 0; //Se creó la cuenta pero no se envió el mail
            Account account = new Account();
            //MailHelper.Mail mail = new MailHelper.Mail("correos.alave@outlook.es", "jmcmxgmd00");
            #endregion
            #region CollectedData
            try { account.data.name = Request.Form.Get("names"); } catch { };
            try { account.data.lastname = Request.Form.Get("lastname"); } catch { };
            try { account.data.email = Request.Form.Get("mail"); } catch { };
            try { account.data.cellphone = Request.Form.Get("cellphone"); } catch { };
            try { account.data.phone = Request.Form.Get("phone"); } catch { };
            try { account.data.idfromproject = int.Parse(Request.Form.Get("idfromproject")); } catch { };
            var comesfrom = (Account)Session["accountForConvert"];
            try { account.data.comesfrom = comesfrom.data.comesfrom; } catch { }
            if (comesfrom.data.comesfrom != "Gamasis" || comesfrom.data.comesfrom != "BioMatcher")
                account.data.rol = 2; //Customer
            else
                account.data.rol = 1;//ADMIN
            try { account.assignations = comesfrom.assignations; } catch { } //En caso de contar con asignaciones
            try { account.data.tcn = comesfrom.data.tcn; } catch { } //En caso de contar con tcn
            #endregion
            #region CreateOperation
            account.id = PMAccount.insertalfas(account.data);
            #endregion
            #region AssignationsByDefault
            if (account.id != 0)
            {
                if (account.assignations.Count > 0)
                {
                    foreach (Assignation i in account.assignations)
                    {
                        PMAccount.setAsignation(i, account.id);
                    }
                }
            }
            #endregion
            #region SendMail
            if (account.id != 0)
                res = ActivateMail(account);
            else
                res = 2; //No se creó la cuenta
            #endregion
            return res;
        }
        public int ActivateMail(Account account)
        {
            #region conditions
            if (account.id == 0)
            {
                account = (Account)Session["accountForActivate"];
                account.data.email = Request.Form.Get("mail");
                try { PMAccount.uptalfas(account.data, account.id); } catch { }
            }
            #endregion
            #region Attr
            int res = 0;
            MailHelper.Mail mail = new MailHelper.Mail("correos.alave@outlook.es", "jmcmxgmd00");
            #endregion
            #region CreateActiveMail
            StringWriter writer = new StringWriter();
            HtmlTextWriter html = new HtmlTextWriter(writer);
            mail.body = "";
            mail.subject = "Activación de cuenta de usuario, Gamasis Project Management - " + account.data.name + " " + account.data.lastname + " del proyecto " + account.data.comesfrom;
            html.RenderBeginTag(HtmlTextWriterTag.P);
            html.AddAttribute(HtmlTextWriterAttribute.Src, "http://gamasis.com.mx/Content/Images/GamasisGrayBlue.png");
            html.AddAttribute(HtmlTextWriterAttribute.Width, "340");
            html.AddAttribute(HtmlTextWriterAttribute.Height, "100");
            html.RenderBeginTag(HtmlTextWriterTag.Img);
            html.RenderEndTag();
            html.RenderEndTag();
            html.RenderBeginTag(HtmlTextWriterTag.H1);
            html.WriteEncodedText("Activa tu cuenta de usuario");
            html.RenderEndTag();

            html.WriteEncodedText(String.Format("Estimado {0},", account.data.name + " " + account.data.lastname));
            html.WriteBreak();
            html.RenderBeginTag(HtmlTextWriterTag.P);
            html.WriteEncodedText("El motivo de este correo, es para la activación de tu cuenta: " + account.username + " en el sistema de administración de proyectos (Gamasis Project Management)  con fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:hh:mm") + "\n");
            html.RenderEndTag();
            html.RenderBeginTag(HtmlTextWriterTag.P);
            html.WriteEncodedText("Para poder activar tu cuenta, da click en el link adjunto:\n");
            html.RenderEndTag();
            html.RenderBeginTag(HtmlTextWriterTag.Strong);
            html.RenderBeginTag(HtmlTextWriterTag.A);

            html.WriteEncodedText(string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~")) + "Authenticate/Activate?updi=" + account.id);
            html.RenderEndTag();
            html.RenderEndTag();
            html.RenderBeginTag(HtmlTextWriterTag.P);
            html.WriteEncodedText("     \n Por su atención, gracias y no responda a este email.");
            html.RenderEndTag();
            html.Flush();

            mail.body = writer.ToString();
            #endregion
            #region ActiveMailAttr
            mail.server.port = 587;
            mail.server.server = "smtp-mail.outlook.com";
            mail.emaillist.Add(account.data.email);
            #endregion
            #region SendActiveMail
            res = MailHelper.send(mail);
            #endregion
            return res;
        }
        public ActionResult Activate()
        {
            int upid = 0, res = 0;
            try { upid = int.Parse(Request.QueryString.Get("updi")); } catch { return RedirectToAction("Index", "Authenticate"); }
            #region ActivateOperation
            res = PMAccount.setactive(upid);
            #endregion
            ViewBag.res = res;
            return View();
        }
        public ActionResult ActivateAccount()
        {
            if (Session["accountForActivate"] == null)
                return RedirectToAction("Index", "Authenticate");
            Account account = (Account)Session["accountForActivate"];
            ViewBag.account = account;
            return View();
        }
        public int endSession()
        {
            int res = 1;
            Session.Remove("GNT_SESSION");
            return res;
        }
    }
}