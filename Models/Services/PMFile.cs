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
    public class PMFile
    {

        public static List<File> getfiles(int idinc, int type = 0, bool justids = false) //enum('Incident','Module')
        {
            List<File> res = new List<File>();
            string query = "SELECT * FROM files where idrel=" + idinc + ";";
            if (type != 0)
                query = "SELECT * FROM files where idrel=" + idinc + " AND type=" + type + " ;";
            DataTable dt = new DataTable();
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var f = new File();
                    if (justids == false)
                        f.data = (byte[])dr["document"];
                    f.idrel = int.Parse(dr["idrel"].ToString());
                    f.id = int.Parse(dr["idfile"].ToString());
                    f.extension = dr["extension"].ToString();
                    f.name = dr["name"].ToString();
                    f.mime = dr["mimetype"].ToString();
                    res.Add(f);
                }
            }
            return res;
        }
        public static File getfile(int id, int type = 1) //enum('Incident','Module')
        {
            var res = new File();
            string query = "SELECT * FROM files where idfile=" + id + " AND type=" + type + " ;";
            DataTable dt = new DataTable();
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    res.data = (byte[])dr["document"];
                    res.idrel = int.Parse(dr["idrel"].ToString());
                    res.id = int.Parse(dr["idfile"].ToString());
                    res.extension = dr["extension"].ToString();
                    res.name = dr["name"].ToString();
                    res.mime = dr["mimetype"].ToString();
                }
            }
            return res;
        }
        public static int remove(int idfile)
        {
            int res = 0;
            string query = string.Format("DELETE FROM files where idfile={0};", idfile);
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al eliminar el requerimiento {0}: {1} ", idfile, ex.Message)); }
            return res;
        }
        public static int addFile(File f, int type = 1) //enum('Incident','Module')
        {
            int res = 0;
            string query = "INSERT INTO files " +
                          "(name, document, mimetype,extension,type, idrel) VALUES(" +
                          "'" + f.name + "'," +
                          "@doc," +
                          "'" + f.mime + "'," +
                          "'" + f.extension + "'," +
                          "" + f.type + "," +
                          "'" + f.idrel + "'" +
                          ")";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@doc", f.data);
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb"), parameters); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al añadir un archivo: {0} ", ex.Message)); }
            return res;
        }

        public static int addFiles(List<File> fs) //enum('Incident','Module')
        {
            int res = 0;
            string query = "";
            foreach (File f in fs)
            {
                query = "INSERT INTO files " +
                        "(name, document, mimetype,extension,type, idrel) VALUES(" +
                        "'" + f.name + "'," +
                        "@doc," +
                        "'" + f.mime + "'," +
                        "'" + f.extension + "'," +
                        "" + f.type + "," +
                        "'" + f.idrel + "'" +
                        ")";
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("@doc", f.data);
                try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb"), parameters); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al añadir un archivo: {0} ", ex.Message)); }
            }
            return res;
        }
        public static int removebyrel(int idrel, int type) //1,2
        {
            int res = 0;
            string query = string.Format("DELETE FROM files where idrel={0} AND type={1};", idrel, type);
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al eliminar el requerimiento {0}: {1} ", idrel, ex.Message)); }
            return res;
        }
    }
}