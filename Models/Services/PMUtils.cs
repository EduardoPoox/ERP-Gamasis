using Gamasis.ProjectManagement.Classes;
using Gamasis.ProjectManagement.Models.Objects;
using Gamasis.ProjectManagement.Models.Objects.Views;
using Gamasis.Utils.MySql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Gamasis.ProjectManagement.Models.Services
{
    public class PMUtils
    {
        public static Dashboard Dashboard()
        {
            Dashboard res = new Objects.Views.Dashboard();
            Account currentaccount = SessionHelper.Account();
            DataTable dt = new DataTable();
            if (currentaccount.id == 0)
                return res;
            string comesfrom = string.Format("WHERE ad.comesfrom='{0}'", currentaccount.data.comesfrom);
            string comesfromconcluded = string.Format("WHERE STATUS=5 AND ad.comesfrom='{0}'", currentaccount.data.comesfrom);
            if (currentaccount.rol == 3)
            {
                comesfrom = "WHERE ";
                comesfromconcluded = "WHERE STATUS=5 AND (";
                foreach (Assignation i in currentaccount.assignations)
                {
                    if (i.type == 2)
                    {
                        comesfrom += string.Format("ad.comesfrom='{0}' OR", i.value);
                        comesfromconcluded += string.Format("ad.comesfrom='{0}' OR", i.value);
                    }
                }
                comesfrom = comesfrom.Substring(0, comesfrom.Length - 2) + ")";
            }
            else if (currentaccount.rol == 1)
            {
                comesfrom = "";
                comesfromconcluded = "WHERE STATUS=5";
            }

            string query = string.Format("SELECT ifnull(COUNT(i.idincident), 0) as total FROM incident i JOIN alfas_data ad on ad.iddata=i.idaccount {0};", comesfrom);
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            if (dt.Rows.Count > 0)
                res.reportedincindences = int.Parse(dt.Rows[0]["total"].ToString());
            query = string.Format("SELECT ifnull(COUNT(i.idincident), 0) as total FROM incident i JOIN alfas_data ad on ad.iddata=i.idaccount  {0};", comesfromconcluded);
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            if (dt.Rows.Count > 0)
                res.concludedincindences = int.Parse(dt.Rows[0]["total"].ToString());
            query = string.Format("SELECT ifnull(COUNT(m.idmodule), 0) as total FROM module m JOIN alfas_data ad on ad.iddata=m.idaccount {0};", comesfrom);
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            if (dt.Rows.Count > 0)
                res.requestedmodules = int.Parse(dt.Rows[0]["total"].ToString());
            query = string.Format("SELECT ifnull(COUNT(m.idmodule), 0) as total FROM module m JOIN alfas_data ad on ad.iddata=m.idaccount {0};", comesfromconcluded);
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            if (dt.Rows.Count > 0)
                res.concludedmodules = int.Parse(dt.Rows[0]["total"].ToString());
            try { res.concludedmodulepercent = ((res.concludedmodules * 100) / res.requestedmodules); } catch { }
            try { res.concludedincindentpercent = ((res.concludedincindences * 100) / res.reportedincindences); } catch { }

            try { res.inclist = PMIncident.get(SessionHelper.Account(), true); } catch { }
            try { res.reqlist = PMRequirement.get(SessionHelper.Account(), true); } catch { }

            return res;
        }
        public static int insertlog(Log log, string platform = "WEB")
        {
            int res = 0;
            int platfominteger = 1;

            switch (platform)
            {
                case "WEB":
                    platfominteger = 1;
                    break;
                case "DESKTOP":
                    platfominteger = 2;
                    break;
                case "MOBILE":
                    platfominteger = 3;
                    break;
            }
            string query = "INSERT INTO `gpm`.`account_log` " +
                "(date,ip,accountid,platform,val,idrel,comment,navigator" + ((log.subtype != 0) ? ",subtype" : "") + ",type) VALUES (" +
                //"sysdate()" + "," +
                "'" + log.date + "'," +
                "'" + log.ip + "'," +
                "'" + log.accountid + "'," +
                "'" + platfominteger + "'," +
                "'" + log.val + "'," +
                "" + log.idrel + "," +
                "'" + log.comments + "'," +
                "'" + log.navigator + "'," +
                ((log.subtype != 0) ? log.subtype.ToString() + "," : "") +
                "" + log.type + ") ;";  //Access, Status, Incident, Requirement
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al registar el log: {0} ", ex.Message)); }
            return res;
        }
        public static List<string> getMails(string company, int type) //Tipo programador = 1, Tipo cliente = 2
        {
            DataTable dt = new DataTable();
            List<string> mails = new List<string>();
            string query = string.Format("SELECT ad.email FROM alfas_data ad where ad.comesfrom='{0}';", company);
            if (type == 1) //Programador
            {
                query = string.Format("SELECT ad.email FROM alfas_data ad JOIN assignations a on a.iddata=ad.iddata where a.type=2 AND a.name='{0}';", company);
            }
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            foreach (DataRow i in dt.Rows)
            {
                mails.Add(i["email"].ToString());
            }
            return mails;
        }
        public static List<string> getMails(int iddl)
        {
            DataTable dt = new DataTable();
            List<string> mails = new List<string>();
            string query = string.Format("SELECT distinct ad.email FROM cross_dl_member cdlm LEFT JOIN alfas_data ad on ad.iddata = cdlm.idmember WHERE idrel={0};", iddl);
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            foreach(DataRow dr in dt.Rows)
            {
                mails.Add(dr["email"].ToString());
            }
            return mails;
        }


    }
}