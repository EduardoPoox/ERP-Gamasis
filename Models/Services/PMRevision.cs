using Gamasis.ProjectManagement.Classes;
using Gamasis.ProjectManagement.Models.Objects;
using Gamasis.Utils.MySql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Gamasis.ProjectManagement.Models.Services
{
    public class PMRevision
    {
        public static int create(Revision rev)
        {
            int res = 0;
            string query = string.Format("INSERT INTO revision (name, date, userid, idrel, type, comment) values('{0}','{1}',{2},{3},{4},'{5}');", rev.name, rev.date, rev.accountid, rev.idrel, rev.type, rev.comment);
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            return res;
        }
        public static List<Revision> get(int idrel, int type = 1) //1 Incident, 2 Req
        {
            List<Revision> res = new List<Revision>();
            DataTable dt = new DataTable();
            string query = string.Format("SELECT r.idrevision, r.name, r.userid, r.date, r.idrel, CAST(r.type AS UNSIGNED) as type, r.comment, concat_ws(' ',ad.name, ad.lastname) as fullname FROM revision r LEFT JOIN alfas_data ad on ad.iddata = r.userid WHERE r.idrel={0} AND r.type={1}", idrel, type);
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow rev in dt.Rows)
                {
                    var obj = new Revision();
                    obj.id = int.Parse(rev["idrevision"].ToString());
                    obj.type = int.Parse(rev["type"].ToString());
                    obj.name = rev["name"].ToString();
                    obj.comment = rev["comment"].ToString();
                    obj.date = DateTime.Parse(rev["date"].ToString()).ToString("dd/MM/yyyy HH:mm");
                    obj.accountname = rev["fullname"].ToString();
                    obj.accountid = int.Parse(rev["userid"].ToString());
                    obj.idrel = int.Parse(rev["idrel"].ToString());
                    res.Add(obj);
                }
            }
            return res;
        }
        public static int remove(int idrev)
        {
            int res = 0;
            string query = string.Format("DELETE FROM revision where idrevision={0};", idrev);
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al eliminar la revisión {0}: {1} ", idrev, ex.Message)); }
            return res;
        }
        public static int removebyrel(int idrel, int type) //1,2
        {
            int res = 0;
            string query = string.Format("DELETE FROM revision where idrel={0} AND type={1};", idrel, type);
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al eliminar las revisiones de la relación {0}: {1} ", idrel, ex.Message)); }
            return res;

        }
    }
}