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
    public class PMDistribution
    {
        public static int Add(DistributionList dist)
        {
            int res = 0;
            int id = 0;
            string query = string.Format("INSERT INTO distributionlist(name, description) VALUES('{0}','{1}');", dist.name, dist.description);
            try { id = SQL_Queries.QueryInsertAndGetId(query, ConnectionHelper.getConnString("gpmdb")); } catch(Exception ex) { }
            if (id != 0)
            {
                foreach (Member m in dist.members)
                {
                    query = string.Format("INSERT INTO cross_dl_member(idrel, idmember) VALUES('{0}','{1}');", id, m.id);
                    try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex){ }
                }
            }
            return res;
        }
        public static int Update(DistributionList dist)
        {
            int res = 0;
            int id = 0;

            string query = "UPDATE distributionlist SET " +
               (!string.IsNullOrEmpty(dist.name) ? "name='" + dist.name + "', " : "") +
               (!string.IsNullOrEmpty(dist.description) ? "description='" + dist.description + "', " : "");
            query = query.Trim();
            query = query.TrimEnd(',');
            query += " WHERE id=" + dist.id + ";";
            try { id = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            if (id != 0)
            {
                query = string.Format("DELETE FROM cross_dl_member WHERE idrel={0};", dist.id);
                try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
                foreach (Member m in dist.members)
                {
                    query = string.Format("INSERT INTO cross_dl_member(idrel, idmember) VALUES('{0}','{1}');", dist.id, m.id);
                    try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
                }
            }
            return res;
        }

        public static List<DistributionList> Get()
        {
            List<DistributionList> res = new List<DistributionList>();
            DataTable dt = new DataTable();
            string query = string.Format("SELECT * FROM distributionlist;");
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            foreach (DataRow item in dt.Rows)
            {
                var obj = new DistributionList();
                obj.id = int.Parse(item["id"].ToString());
                obj.name = item["name"].ToString();
                obj.description = item["description"].ToString();
                obj.members = GetMembers(obj.id);
                res.Add(obj);
            }
            return res;
        }
        public static DistributionList GetOne(int id)
        {
            DistributionList res = new DistributionList();
            DataTable dt = new DataTable();
            string query = string.Format("SELECT * FROM distributionlist WHERE id={0};", id);
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            foreach (DataRow item in dt.Rows)
            {
                res.id = int.Parse(item["id"].ToString());
                res.name = item["name"].ToString();
                res.description = item["description"].ToString();
                res.members = GetMembers(res.id);
            }
            return res;
        }
        public static List<Member> GetMembers(int idrel)
        {
            List<Member> res = new List<Member>();
            DataTable dt = new DataTable();
            string query = string.Format("SELECT cdlm.idcdlm, cdlm.idrel, cdlm.idmember, concat_ws(' ', ad.name, ad.lastname) as name, ad.email, ad.cellphone, ad.comesfrom FROM cross_dl_member cdlm LEFT JOIN alfas_data ad on cdlm.idmember = ad.iddata WHERE cdlm.idrel={0};", idrel);
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            foreach (DataRow i in dt.Rows)
            {
                res.Add(new Member()
                {
                    id = int.Parse(i["idcdlm"].ToString()),
                    idrel = idrel,
                    name = i["name"].ToString(),
                    mail = i["email"].ToString(),
                    comesfrom = i["comesfrom"].ToString(),
                    idmember = int.Parse(i["idmember"].ToString()),
                    cellphone = i["cellphone"].ToString()
                });
            }
            return res;
        }
    }
}