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
    public class PMRequirement
    {
        public static int create(Requirement req)
        {
            int res = 0;
            string query = "INSERT INTO module " +
                "(name, description, date, priority, status, ubication, idaccount, type, template, columnsno, progress) VALUES(" +
                "'" + req.name + "'," +
                "'" + req.description + "'," +
                "'" + req.date + "'," +
                "" + req.priority + "," +
                "" + req.status + "," +
                "'" + req.ubication + "'," +
                "'" + req.idaccount + "'," +
                "" + req.type + "," +
                "'" + req.template.Trim().Replace("\r\n", "").Replace("\n", "").Replace("\r", "") + "'," +
                "'" + req.columns + "'," +
                "'" + req.progress + "'" +
                ")";
            try { res = SQL_Queries.QueryInsertAndGetId(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al añadir una incidencia: {0} ", ex.Message)); }
            if (res != 0)
            {
                if (req.files.Count > 0)
                {
                    foreach (File f in req.files)
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
        public static int update(Requirement req, bool acceptZero = false)
        {
            int res = 0;
            string query = "UPDATE module SET " +
                (req.status != 0 ? "status=" + req.status + ", " : "") +
                (req.statussubtype != 0 ? "subtype=" + req.statussubtype + ", " : "subtype='" + req.statussubtype + "', ") +
                (!string.IsNullOrEmpty(req.description) ? "description=" + req.description + ", " : "") +
                (!string.IsNullOrEmpty(req.name) ? "name=" + req.name + ", " : "") +
                (req.priority != 0 ? "priority=" + req.priority + ", " : "") +
                (!string.IsNullOrEmpty(req.ubication) ? "ubication=" + req.ubication + ", " : "") +
                ((acceptZero == true ? "progress=" + req.progress + ", " : req.progress != 0 ? "progress=" + req.progress + ", " : ""));
            query = query.Trim();
            query = query.TrimEnd(',');
            query += " WHERE idmodule=" + req.id + ";";
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al añadir una incidencia: {0} ", ex.Message)); }
            return res;
        }
        public static List<Requirement> get(Account session, bool byid = false)
        {
            List<Requirement> res = new List<Requirement>();

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

            string query = "SELECT i.template, i.concludedby, i.concludeddate, CAST(i.subtype AS UNSIGNED) as subtype, concat_ws(' ',ad.name,ad.lastname) as accountName, CAST(i.type AS UNSIGNED) as type, i.ubication,i.progress,i.idaccount, i.idmodule,i.name, i.description, i.date, CAST(i.priority AS UNSIGNED) as priority, CAST(i.status AS UNSIGNED) as status  " +
                "FROM module i  " +
                "JOIN alfas_data ad on ad.iddata = i.idaccount " +
                ((byid == true) ? ((session.id == 0) ? " ORDER BY status; " : "WHERE i.idaccount=" + session.id + " ORDER BY status;") : string.Format("{0} ORDER BY status;", comesfrom));
            DataTable dt = new DataTable();
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    var i = new Requirement();
                    i.name = r["name"].ToString();
                    i.description = r["description"].ToString();
                    try { i.date = Convert.ToDateTime(r["date"].ToString()).ToShortDateString(); } catch { }
                    i.priority = int.Parse(r["priority"].ToString());
                    i.status = int.Parse(r["status"].ToString());
                    i.ubication = r["ubication"].ToString();
                    i.progress = int.Parse(r["progress"].ToString());
                    i.id = int.Parse(r["idmodule"].ToString());
                    i.idaccount = int.Parse(r["idaccount"].ToString());
                    try { i.concludedbyid = int.Parse(r["concludedby"].ToString()); } catch { }
                    try { i.concludeddate = Convert.ToDateTime(r["concludeddate"].ToString()).ToShortDateString(); } catch { }
                    try { i.statussubtype = int.Parse(r["subtype"].ToString()); } catch { }
                    i.type = int.Parse(r["type"].ToString());
                    i.accountName = r["accountName"].ToString();
                    i.template = @r["template"].ToString().Trim().Replace("\r\n", "").Replace("\n", "").Replace("\r", "");
                    i.files = PMFile.getfiles(i.id, 2, true);
                    i.programmers = PMProgrammer.getprogrammers(i.id, "req");
                    res.Add(i);
                }
            }
            return res;
        }
        public static Requirement getone(int idreq = 0)
        {
            Requirement res = new Requirement();
            string query = "SELECT i.template, i.concludedby, i.concludeddate, CAST(i.subtype AS UNSIGNED) as subtype, ad.email, ad.cellphone, concat_ws(' ',ad.name,ad.lastname) as accountName, i.ubication,i.progress,i.idaccount, i.idmodule,i.name, i.description, i.date, CAST(i.priority AS UNSIGNED) as priority, CAST(i.type AS UNSIGNED) as type, CAST(i.status AS UNSIGNED) as status  " +
                "FROM module i  " +
                "JOIN alfas_data ad on ad.iddata = i.idaccount " +
                "WHERE i.idmodule=" + idreq + " ORDER BY status;";
            DataTable dt = new DataTable();
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    res.name = r["name"].ToString();
                    res.description = r["description"].ToString();
                    try { res.date = Convert.ToDateTime(r["date"].ToString()).ToString("dd/MM/yyyy HH:mm"); } catch { }
                    res.priority = int.Parse(r["priority"].ToString());
                    res.status = int.Parse(r["status"].ToString());
                    res.ubication = r["ubication"].ToString();
                    res.accountMail = r["email"].ToString();
                    res.accountPhone = r["cellphone"].ToString();
                    res.progress = int.Parse(r["progress"].ToString());
                    res.id = int.Parse(r["idmodule"].ToString());
                    res.type = int.Parse(r["type"].ToString());
                    res.idaccount = int.Parse(r["idaccount"].ToString());
                    try { res.concludedbyid = int.Parse(r["concludedby"].ToString()); } catch { }
                    try { res.concludeddate = Convert.ToDateTime(r["concludeddate"].ToString()).ToShortDateString(); } catch { }
                    try { res.statussubtype = int.Parse(r["subtype"].ToString()); } catch { }
                    res.accountName = r["accountName"].ToString();
                    res.template = r["template"].ToString().Trim().Replace("\r\n", "").Replace("\n", "").Replace("\r", "");
                    res.files = PMFile.getfiles(res.id, 2, true);
                    res.programmers = PMProgrammer.getprogrammers(res.id, "req");
                    res.revisions = PMRevision.get(res.id, 2);
                    res.feedbacks = PMFeedback.get(res.id, 2);
                    res.comments = PMComment.get(res.id, 2);
                }
            }
            return res;
        }
        public static int setConcluded(int idby, int idinc, string date)
        {
            int res = 0;
            string query = string.Format("UPDATE module set status=5, concludedby={0}, concludeddate='{2}' where idmodule={1};", idby, idinc, date);
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al cambiar el status del requerimiento con id {0}: {1} ", idinc, ex.Message)); }
            return res;
        }
        public static int setCoding(int idby, int idinc)
        {
            int res = 0;
            string query = string.Format("UPDATE module set status=3, progress=0, subtype=2 where idmodule={0};", idinc);
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al cambiar el status del requerimiento con id {0}: {1} ", idinc, ex.Message)); }
            return res;
        }
        public static int remove(int idreq)
        {
            int res = 0;
            string query = string.Format("DELETE FROM module where idmodule={0};", idreq);
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al eliminar el requerimiento {0}: {1} ", idreq, ex.Message)); }
            if (res == 1)
            {
                #region Files
                PMFile.removebyrel(idreq, 2);
                #endregion
                #region Programmers
                PMProgrammer.removebyrel(idreq, "req");
                #endregion
                #region Revisions
                PMRevision.removebyrel(idreq, 2);
                #endregion
            }
            return res;
        }
    }
}