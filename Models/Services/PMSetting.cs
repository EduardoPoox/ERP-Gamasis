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
    public class PMSetting
    {

        public static List<Setting> Get(string setting, int type)
        {
            List<Setting> res = new List<Setting>();
            DataTable dt = new DataTable();
            string query = string.Format("SELECT id, idrel, CAST(type AS UNSIGNED) as type, setting, value FROM gpm.settings WHERE setting='{0}' AND type={1};", setting, type);
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow i in dt.Rows)
                {
                    var sett = new Setting();
                    sett.id = int.Parse(i["id"].ToString());
                    try { sett.idrel = int.Parse(i["idrel"].ToString()); } catch { }
                    sett.type = int.Parse(i["id"].ToString());
                    sett.setting = i["id"].ToString();
                    sett.value = i["value"].ToString();
                    res.Add(sett);
                }
            }
            return res;
        }
    }
}