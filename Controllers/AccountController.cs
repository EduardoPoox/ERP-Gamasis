using Gamasis.ProjectManagement.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gamasis.ProjectManagement.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            ViewBag.Users = PMAccount.Get(SessionHelper.Account().data.comesfrom, SessionHelper.Account().rol);
            return View();
        }
    }
}