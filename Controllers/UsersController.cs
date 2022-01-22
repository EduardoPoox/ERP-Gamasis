using Gamasis.ProjectManagement.Models.Objects;
using Gamasis.ProjectManagement.Models.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gamasis.ProjectManagement.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        [Authenticate(module = "Users")]
        public ActionResult Index()
        {
            ViewBag.UserLogged = SessionHelper.Account();
            ViewBag.Users = PMAccount.Get();
            ViewBag.Roles = PMRol.Get();
            return View();
        }
        [Authenticate]
        public JsonResult GetOne()
        {
            int id = 0;
            try { id = int.Parse(Request.QueryString.Get("id")); } catch { }
            return Json(PMAccount.GetOne(id), "application/json", JsonRequestBehavior.AllowGet);
        }
        public int Update()
        {
            #region attr
            int res = 0;
            var alfas = new Alfas();
            List<Assignation> assignations = new List<Assignation>();
            #endregion
            #region recollectedData
            alfas.name = Request.Form.Get("name");
            alfas.lastname = Request.Form.Get("lastname");
            alfas.email = Request.Form.Get("mail");
            alfas.cellphone = Request.Form.Get("cellphone");
            alfas.rol = int.Parse(Request.Form.Get("nrol"));
            try { assignations = JsonConvert.DeserializeObject<List<Assignation>>(System.Web.Helpers.Json.Decode(JsonConvert.SerializeObject(Request.Form.Get("dlassig")))); } catch (Exception ex) { }
            try { alfas.iddata = int.Parse(Request.Form.Get("_id")); } catch { }

            #endregion
            #region operation
            res = PMAccount.uptalfas(alfas, alfas.iddata);

            if(res > 0)
            {
                PMAccount.deleteAssignations(alfas.iddata);

                foreach(Assignation ass in assignations)
                {
                    PMAccount.setAsignation(ass, alfas.iddata);
                }
            }

            #endregion
            return res;
        }



        [Authenticate(module = "Users")]
        public ActionResult Distribution()
        {
            ViewBag.UserLogged = SessionHelper.Account();
            ViewBag.Users = PMAccount.Get();
            ViewBag.Distlist = PMDistribution.Get();
            return View();
        }
        [Authenticate]
        public int AddDist()
        {
            #region attr
            var res = 0;
            DistributionList list = new DistributionList();
            #endregion
            #region recollectedData
            try { list.description = Request.Form.Get("description"); } catch { }
            try { list.name = Request.Form.Get("name"); } catch { }
            try { list.members = JsonConvert.DeserializeObject<List<Member>>(System.Web.Helpers.Json.Decode(JsonConvert.SerializeObject(Request.Form.Get("dlmembers")))); } catch (Exception ex) { }
            #endregion
            #region Operation
            res = PMDistribution.Add(list);
            #endregion
            return res;
        }
        [Authenticate]
        public int UpdateDist()
        {
            #region attr
            var res = 0;
            DistributionList list = new DistributionList();
            #endregion
            #region recollectedData
            try { list.id = int.Parse(Request.Form.Get("_id")); } catch { }
            try { list.description = Request.Form.Get("description"); } catch { }
            try { list.name = Request.Form.Get("name"); } catch { }
            try { list.members = JsonConvert.DeserializeObject<List<Member>>(System.Web.Helpers.Json.Decode(JsonConvert.SerializeObject(Request.Form.Get("dlmembers")))); } catch (Exception ex) { }
            #endregion
            #region Operation
            res = PMDistribution.Update(list);
            #endregion
            return res;
        }
        [Authenticate]
        public JsonResult GetDist()
        {
            int id = 0;
            try { id = int.Parse(Request.QueryString.Get("id")); } catch { }
            return Json(PMDistribution.GetOne(id), "application/json", JsonRequestBehavior.AllowGet);
        }




    }
}