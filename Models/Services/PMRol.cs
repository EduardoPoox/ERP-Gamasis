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
    public class PMRol
    {
        public static List<Rol> Get()
        {
            List<Rol> res = new List<Rol>();
            DataTable dt = new DataTable();
            string query = string.Format("SELECT * FROM account_roles;");
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            foreach (DataRow i in dt.Rows)
            {
                var obj = new Rol();
                obj.code = i["code"].ToString();
                obj.id = int.Parse(i["idrole"].ToString());
                obj.description = i["description"].ToString();
                res.Add(obj);
            }
            return res;
        }
        public static Rol GetOne(int id)
        {
            Rol res = new Rol();
            DataTable dt = new DataTable();
            string query = string.Format("SELECT * FROM account_roles WHERE idrole={0};", id);
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            foreach (DataRow i in dt.Rows)
            {
                res.code = i["code"].ToString();
                res.id = int.Parse(i["idrole"].ToString());
                res.description = i["description"].ToString();
            }
            return res;
        }

    }
}