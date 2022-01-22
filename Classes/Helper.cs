using Gamasis.ProjectManagement.Models.Objects;
using System;
using System.Web;
using System.Web.Mvc;
public static class Url
{
    public static string GetAppPath()
    {
        return HttpContext.Current.Server.MapPath("~/");
    }

    public static string GetLogsPath()
    {
        return string.Format("{0}\\Logs", GetAppPath());
    }
    /// <summary>
    /// Return's string with absolute path
    /// </summary>
    /// <param name="Path"></param>
    /// <returns></returns>
    public static string Content(string Path)
    {
        try
        {
            return VirtualPathUtility.ToAbsolute(Path);
        }
        catch
        {
            return Path;
        }
    }
    /// <summary>
    /// Return's system route for controller and action
    /// </summary>
    /// <param name="Action"></param>
    /// <param name="Controller"></param>
    /// <returns></returns>
    public static string Action(string Action, string Controller)
    {
        return Content(string.Format("~/{0}/{1}", Controller, Action));
    }
}
public static class SessionHelper
{
    public static Account Account(string Roles = null)
    {
        Account person = null;
        try
        {
            person = (Account)HttpContext.Current.Session["GNT_SESSION"];
            person = person == null ? new Account() : person;
            person.isactive = person.id != 0 ? true : false;
        }
        catch { }
        return person;
    }
    public static void SetCacheablePhoto()
    {
        var CurrentAccount = SessionHelper.Account();
        if (CurrentAccount != null)
        {
            if (CurrentAccount.isactive != false)
            {
                if (CurrentAccount.avatar != null)
                    try { HttpContext.Current.Cache.Insert(CurrentAccount.id.ToString(), CurrentAccount.avatar); } catch (Exception ex) { throw ex; }
                else
                    try { byte[] Face = null; HttpContext.Current.Cache.Insert(CurrentAccount.id.ToString(), Face); } catch (Exception ex) { throw ex; }
            }
        }
    }
}
public class Authenticate : AuthorizeAttribute
{
    protected virtual Account CurrentUser
    {
        get { return SessionHelper.Account() as Account; }
    }
    public string module { set; get; }
    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
        bool flag = false;
        if (CurrentUser.id != 0)
        {
            flag = true;
            if (CurrentUser.rol == 2)
            {
                if (!string.IsNullOrEmpty(module))
                {
                    if (CurrentUser.hasAccessToModule(module))
                        flag = true;
                    else
                        flag = false;
                }
            }
        }

        return flag;
    }
    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        RedirectToRouteResult routeData = null;

        if (CurrentUser.id == 0)
        {
            routeData = new RedirectToRouteResult
                (new System.Web.Routing.RouteValueDictionary
                (new
                {
                    controller = "Authenticate",
                    action = "Index",
                }
                ));
        }
        else
        {
            routeData = new RedirectToRouteResult
            (new System.Web.Routing.RouteValueDictionary
             (new
             {
                 controller = "Home",
                 action = "Index"
             }
             ));
        }
        filterContext.Result = routeData;
    }
}

