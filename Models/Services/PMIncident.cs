using Gamasis.ProjectManagement.Classes;
using Gamasis.ProjectManagement.Models.Objects;
using Gamasis.Utils.MySql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static Gamasis.ProjectManagement.Models.Objects.Incident;

namespace Gamasis.ProjectManagement.Models.Services
{
    public class PMIncident
    {
        public static int create(Incident incident)
        {
            int res = 0;
            string query = "INSERT INTO incident " +
                "(name, description, date, priority, status, ubication, idaccount, progress) VALUES(" +
                "'" + incident.name + "'," +
                "'" + incident.description + "'," +
                "sysdate()," +
                "'" + incident.priority + "'," +
                "'" + incident.status + "'," +
                "'" + incident.ubication + "'," +
                "'" + incident.idaccount + "'," +
                "'" + incident.progress + "'" +
                ")";
            try { res = SQL_Queries.QueryInsertAndGetId(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al añadir una incidencia: {0} ", ex.Message)); }

            if (res != 0)
            {
                if (incident.files.Count > 0)
                {
                    foreach (File f in incident.files)
                    {
                        query = "INSERT INTO files " +
                         "(name, document, mimetype,extension,type, idrel) VALUES(" +
                         "'" + f.name + "'," +
                         "@doc," +
                         "'" + f.mime + "'," +
                         "'" + f.extension + "'," +
                         "" + f.type + "," +
                         "'" + res + "'" +
                         ")";
                        Dictionary<string, object> parameters = new Dictionary<string, object>();
                        parameters.Add("@doc", f.data);
                        try { SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb"), parameters); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al añadir un archivo: {0} ", ex.Message)); }
                    }
                }
            }
            return res;
        }

        internal static dynamic getUbications(string comesfrom)
        {
            throw new NotImplementedException();
        }

        public static int update(Incident incident, bool acceptZero = false)
        {
            int res = 0;
            string query = "UPDATE incident SET " +
                (incident.status != 0 ? "status=" + incident.status + ", " : "") +
                (!string.IsNullOrEmpty(incident.description) ? "description=" + incident.description + ", " : "") +
                (!string.IsNullOrEmpty(incident.name) ? "name=" + incident.name + ", " : "") +
                (incident.priority != 0 ? "priority=" + incident.priority + ", " : "") +
                (!string.IsNullOrEmpty(incident.ubication) ? "ubication=" + incident.ubication + ", " : "") +
                ((acceptZero == true ? "progress=" + incident.progress + ", " : incident.progress != 0 ? "progress=" + incident.progress + ", " : ""));
            query = query.Trim();
            query = query.TrimEnd(',');
            query += " WHERE idincident=" + incident.id + ";";

            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al actualizar una incidencia: {0} ", ex.Message)); }
            return res;
        }
        public static List<Incident> get(Account session, bool byid = false)
        {
            List<Incident> res = new List<Incident>();
            string comesfrom = string.Format("WHERE ad.comesfrom='{0}'", session.data.comesfrom);
            if (session.rol == 3)
            {
                comesfrom = "WHERE (";
                foreach (Assignation i in session.assignations)
                {
                    if (i.type == 2)
                        comesfrom += string.Format("ad.comesfrom='{0}' OR", i.value);
                }
                comesfrom = comesfrom.Substring(0, comesfrom.Length - 2) + ")";
            }
            else if (session.rol == 1)
                comesfrom = "";
            string query = "SELECT concat_ws(' ',ad.name,ad.lastname) as accountName, (SELECT concat_ws(' ',adc.name, adc.lastname) FROM alfas_data adc where adc.iddata=i.concludedby) as concludedAccountName, i.ubication,i.progress,i.idaccount, idincident,i.name, i.description, i.date, i.concludeddate, i.concludedby, CAST(i.priority AS UNSIGNED) as priority, CAST(i.status AS UNSIGNED) as status  " +
                "FROM incident i  " +
                "JOIN alfas_data ad on ad.iddata = i.idaccount " +
                ((byid == true) ? ((session.id == 0) ? " ORDER BY status; " : "WHERE i.idaccount=" + session.id + " ORDER BY status;") : string.Format("{0} ORDER BY status;", comesfrom));
            DataTable dt = new DataTable();
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    var i = new Incident();
                    i.name = r["name"].ToString();
                    i.description = r["description"].ToString();
                    i.priority = int.Parse(r["priority"].ToString());
                    i.status = int.Parse(r["status"].ToString());
                    i.ubication = r["ubication"].ToString();
                    i.progress = int.Parse(r["progress"].ToString());
                    i.id = int.Parse(r["idincident"].ToString());
                    try { i.concludedbyid = int.Parse(r["concludedby"].ToString()); } catch { }
                    try { i.date = Convert.ToDateTime(r["date"].ToString()).ToString("dd/MM/yyyy HH:mm"); } catch { }
                    try { i.concludeddate = Convert.ToDateTime(r["concludeddate"].ToString()).ToString("dd/MM/yyyy HH:mm"); } catch { }
                    try { i.concludedby = r["concludedAccountName"].ToString(); } catch { }
                    i.idaccount = int.Parse(r["idaccount"].ToString());
                    i.accountName = r["accountName"].ToString();
                    i.files = PMFile.getfiles(i.id, 1, true);
                    i.programmers = PMProgrammer.getprogrammers(i.id);
                    res.Add(i);
                }
            }
            return res;
        }
        public static Incident getone(int idinc)
        {
            var res = new Incident();
            string query = "SELECT ad.email, (SELECT concat_ws(' ',adc.name, adc.lastname) FROM alfas_data adc where adc.iddata=i.concludedby) as concludedAccountName, ad.cellphone,concat_ws(' ',ad.name,ad.lastname) as accountName, i.ubication,i.progress,i.idaccount, idincident,i.name, i.description, i.date, i.concludeddate, i.concludedby, CAST(i.priority AS UNSIGNED) as priority, CAST(i.status AS UNSIGNED) as status  " +
                "FROM incident i  " +
                "JOIN alfas_data ad on ad.iddata = i.idaccount " +
                ((idinc == 0) ? " ORDER BY status; " : "WHERE i.idincident=" + idinc + " ORDER BY status;");
            DataTable dt = new DataTable();
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    res.name = r["name"].ToString();
                    res.description = r["description"].ToString();
                    try { res.date = Convert.ToDateTime(r["date"].ToString()).ToString("dd/MM/yyyy HH:mm"); } catch { }
                    try { res.concludedbyid = int.Parse(r["concludedby"].ToString()); } catch { }
                    try { res.concludeddate = Convert.ToDateTime(r["concludeddate"].ToString()).ToString("dd/MM/yyyy HH:mm"); } catch { }
                    try { res.concludedby = r["concludedAccountName"].ToString(); } catch { }
                    res.priority = int.Parse(r["priority"].ToString());
                    res.status = int.Parse(r["status"].ToString());
                    res.ubication = r["ubication"].ToString();
                    res.accountMail = r["email"].ToString();
                    res.accountPhone = r["cellphone"].ToString();
                    res.progress = int.Parse(r["progress"].ToString());
                    res.id = int.Parse(r["idincident"].ToString());
                    res.idaccount = int.Parse(r["idaccount"].ToString());
                    res.accountName = r["accountName"].ToString();
                    res.files = PMFile.getfiles(res.id, 1, true);
                    res.programmers = PMProgrammer.getprogrammers(res.id);
                    res.revisions = PMRevision.get(res.id, 1);
                    res.feedbacks = PMFeedback.get(res.id, 1);
                    res.comments = PMComment.get(res.id, 1);
                }
            }
            return res;
        }
        public static int setConcluded(int idby, int idinc, string date)
        {
            int res = 0;
            string query = string.Format("UPDATE incident set status=5, concludedby={0}, concludeddate='{2}' where idincident={1};", idby, idinc, date);
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al cambiar el status a concluido: {0} ", ex.Message)); }
            return res;
        }

        public static int remove(int idinc)
        {
            int res = 0;
            string query = string.Format("DELETE FROM incident where idincident={0};", idinc);
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al eliminar la incidencia {0}: {1} ", idinc, ex.Message)); }
            if (res == 1)
            {
                #region Files
                res = PMFile.removebyrel(idinc, 1);
                #endregion
                #region Revisions
                PMRevision.removebyrel(idinc, 1);
                #endregion
                #region Programmers
                PMProgrammer.removebyrel(idinc, "incident");
                #endregion
            }
            return res;
        }
    }
}