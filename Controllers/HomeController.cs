using Gamasis.ProjectManagement.Models.Objects.Views;
using Gamasis.ProjectManagement.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gamasis.ProjectManagement.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [Authenticate]
        public ActionResult Index()
        {
            ViewBag.dash = PMUtils.Dashboard();
            return View();
        }

    }
} 