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
    public class PMProgrammer
    {
        public static List<Programmer> getprogrammers(int idinc, string type = "inc") //enum('Incident','Module')
        {
            List<Programmer> res = new List<Programmer>();
            string query = "SELECT cip.idincpro,ad.email, ad.cellphone, cip.idpro, cip.idinc, concat_ws(' ',ad.name, ad.lastname) as fullname FROM gpm.cross_incident_programmer cip " +
                "JOIN account a on a.idaccount = cip.idpro " +
                "JOIN alfas_data ad on a.idaccount = ad.iddata WHERE cip.idinc=" + idinc + " ;";

            if (type != "inc")
                query = "SELECT cip.idincpro,ad.email, ad.cellphone, cip.idpro, cip.idreq, concat_ws(' ',ad.name, ad.lastname) as fullname FROM gpm.cross_req_programmer cip " +
                "JOIN account a on a.idaccount = cip.idpro " +
                "JOIN alfas_data ad on a.idaccount = ad.iddata WHERE cip.idreq=" + idinc + " ;";

            DataTable dt = new DataTable();
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var p = new Programmer();
                    p.id = int.Parse(dr["idincpro"].ToString());
                    p.idpro = int.Parse(dr["idpro"].ToString());
                    if (type != "inc")
                        p.idinc = int.Parse(dr["idreq"].ToString());
                    else
                        p.idinc = int.Parse(dr["idinc"].ToString());
                    p.fullname = dr["fullname"].ToString();
                    p.email = dr["email"].ToString();
                    p.cellphone = dr["cellphone"].ToString();
                    res.Add(p);
                }
            }
            return res;
        }
        public static int setprogrammer(int idinc, int idpro, string type = "incident")
        {
            int res = 0;
            string query = "INSERT INTO cross_" + type + "_programmer (" + (type == "req" ? "idreq" : "idinc") + ",idpro) VALUES(" +
                idinc + "," +
                idpro + ");";
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al tomar {0} {1} para el programador {2} :  {3}", type, idinc, idpro, ex.Message)); }
            return res;
        }

        public static int remove(int idpro, string type)
        {
            int res = 0;
            string query = string.Format("DELETE FROM cross_{0}_programmer where idpro={1};", type, idpro);
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al remover al programador {0} de {2} :  {3}", idpro, type, ex.Message)); }
            return res;
        }
        public static int removebyrel(int idrel, string type)
        {
            int res = 0;
            string query = string.Format("DELETE FROM cross_{0}_programmer where {1};", type, (type == "incident" ? "idinc=" + idrel : "idreq=" + idrel));
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al remover a los programadores con relación {0} de {2} :  {3}", idrel, type, ex.Message)); }
            return res;
        }


    }
}