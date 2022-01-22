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
    public class PMFeedback
    {
        public static int create(Feedback fb)
        {
            int res = 0;
            string query = string.Format("INSERT INTO feedback (name, date, userid, idrel, type, comment) values('{0}','{1}',{2},{3},{4},'{5}');", fb.name, fb.date, fb.accountid, fb.idrel, fb.type, fb.comment);
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            return res;
        }
        public static List<Feedback> get(int idrel, int type = 1) //1 Incident, 2 Req
        {
            List<Feedback> res = new List<Feedback>();
            DataTable dt = new DataTable();
            string query = string.Format("SELECT r.idfeedback, r.name, r.userid, r.date, r.idrel, CAST(r.type AS UNSIGNED) as type, r.comment, concat_ws(' ',ad.name, ad.lastname) as fullname FROM feedback r LEFT JOIN alfas_data ad on ad.iddata = r.userid WHERE r.idrel={0} AND r.type={1}", idrel, type);
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow rev in dt.Rows)
                {
                    var obj = new Feedback();
                    obj.id = int.Parse(rev["idfeedback"].ToString());
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
    }
}