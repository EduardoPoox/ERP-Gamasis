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
    public class PMComment
    {
        public static int create(Comment c)
        {
            int res = 0;
            string query = string.Format("INSERT INTO comment (date, userid, idrel, type, comment) values('{0}',{1},{2},{3},'{4}');", c.date, c.accountid, c.idrel, c.type, c.comment);
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            return res;
        }
        public static List<Comment> get(int idrel, int type = 1) //1 Incident, 2 Req
        {
            List<Comment> res = new List<Comment>();
            DataTable dt = new DataTable();
            string query = string.Format("SELECT c.idcomment, c.comment, c.userid, c.date, c.idrel, CAST(c.type AS UNSIGNED) as type, concat_ws(' ',ad.name, ad.lastname) as fullname FROM comment c LEFT JOIN alfas_data ad on ad.iddata = c.userid WHERE c.idrel={0} AND c.type={1}", idrel, type);
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow rev in dt.Rows)
                {
                    var obj = new Comment();
                    obj.id = int.Parse(rev["idcomment"].ToString());
                    obj.type = int.Parse(rev["type"].ToString());
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