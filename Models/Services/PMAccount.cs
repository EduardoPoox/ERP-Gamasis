//using Gamasis.ERP.Clases.Objects;
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
    public class PMAccount
    {
        public static int create(Account account)
        {
            int id = 0;
            int res = 0;
            string query = "INSERT INTO account" +
                "(username, password, idrole, status) VALUES(" +
                "'" + account.username + "'," +
                "AES_ENCRYPT('" + account.password.Trim() + "','gpm.encrypt')," +
                "'" + account.rol + "'," +
                "'" + "2" + "');";
            try { id = SQL_Queries.QueryInsertAndGetId(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al crear la cuenta : {0} ", ex.Message)); }
            if (id != 0)
                res = insertalfas(account.data);
            return id;
        }
        public static int insertalfas(Alfas alfas)
        {
            int res = 0;
            string query = "INSERT INTO alfas_data" +
                "(name, lastname,email,cellphone,phone,tcn,rol,comesfrom,activate,idfromproject) VALUES(" +
                "'" + alfas.name + "'," +
                "'" + alfas.lastname + "'," +
                "'" + alfas.email + "'," +
                "'" + alfas.cellphone + "'," +
                "'" + alfas.phone + "'," +
                "'" + alfas.tcn + "'," +
                "'" + alfas.rol + "'," +
                "'" + alfas.comesfrom + "'," +
                "false," +
                "'" + alfas.idfromproject + "');";
            try { res = SQL_Queries.QueryInsertAndGetId(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al insertar alfas : {0} ", ex.Message)); }
            return res;
        }
        public static int setAsignation(Assignation ass, int idaccount)
        {
            int res = 0;
            string query = string.Format("INSERT INTO assignations (name, iddata, type) values('{0}',{1},{2})", ass.value, idaccount, ass.type);
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            return res;
        }

        public static int deleteAssignations(int iddata)
        {
            int res = 0;
            string query = string.Format("DELETE FROM assignations WHERE iddata={0};", iddata);
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            return res;
        }

        public static int setactive(int id)
        {
            int res = 0;
            string query = "UPDATE alfas_data set activate=true WHERE iddata='" + id + "';";
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al cambiar el status a activo: {0} ", ex.Message)); }
            return res;
        }
        public static int update(Account account)
        {
            int res = 0;
            string query = "UPDATE account SET " +
                 (account.password != "" ? "password= AES_ENCRYPT('" + account.password.Trim() + "','gpm.encrypt'), " : "") +
                 (account.username != "" ? "username='" + account.username + "', " : "") +
                 (account.rol != 0 ? "idrole='" + account.rol + "', " : "") +
                 (account.companyStatus != false ? "companyStatus='1', " : "");
            query = query.Trim();
            query = query.TrimEnd(',');
            query += " WHERE idaccount=" + account.id + "";
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al actualizar la cuenta {1}: {0} ", ex.Message, account.id)); }
            if (res == 1)
            {
                int resAlfas = uptalfas(account.data, account.id);
                if (resAlfas == 1)
                    return res;
                else
                    res = 0;
            }
            return res;
        }
        public static int uptalfas(Alfas data, int idaccount)
        {
            int res = 0;
            string query = "UPDATE alfas_data SET  " +
                    (data.name != "" ? "name='" + data.name + "', " : "") +
                    (data.lastname != "" ? "lastname='" + data.lastname + "', " : "") +
                    (data.email != "" ? "email='" + data.email + "', " : "") +
                    (data.cellphone != "" ? "cellphone='" + data.cellphone + "', " : "") +
                    (data.phone != "" ? "phone='" + data.phone + "', " : "") +
                    (data.tcn != "" ? "tcn='" + data.tcn + "', " : "") +
                    (data.comesfrom != "" ? "comesfrom='" + data.comesfrom + "', " : "") +
                    (data.idfromproject != 0 ? "idfromproject='" + data.idfromproject + "', " : "");

            query = query.Trim();
            query = query.TrimEnd(',');
            query += " WHERE iddata=" + idaccount + ";";
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al actualizar alfas de la cuenta {1}: {0} ", ex.Message, idaccount)); }
            return res;
        }
        public static List<Assignation> GetAssignations(int id)
        {
            DataTable dt = new DataTable();
            List<Assignation> res = new List<Assignation>();
            string query = string.Format("SELECT idassign, ass.iddata, ass.type as stringType,  CAST(ass.type AS UNSIGNED) as type , name as value, (SELECT concat_ws(' ',ad.name,ad.lastname) FROM alfas_data ad where ad.iddata=ass.iddata) as accountname FROM assignations ass WHERE ass.iddata={0} ", id);
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            foreach (DataRow dr in dt.Rows)
            {
                var ass = new Assignation();
                ass.value = dr["value"].ToString();
                ass.id = int.Parse(dr["idassign"].ToString());
                ass.type = int.Parse(dr["type"].ToString());
                ass.accountid = int.Parse(dr["iddata"].ToString());
                ass.accountname = dr["accountname"].ToString();
                ass.typestring = dr["stringType"].ToString();
                res.Add(ass);
            }
            return res;
        }
        public static List<Account> Get(string company, int rol)
        {
            List<Account> res = new List<Account>();
            DataTable dt = new DataTable();
            string query = "SELECT ad.name, ad.lastname,ad.iddata, ad.email, ad.cellphone, ad.comesfrom,ad.tcn, ar.code as rolestring, ad.rol, ad.activate " +
                "FROM alfas_data ad " +
                "LEFT JOIN account_roles ar on ar.idrole = ad.rol " +
                "WHERE ad.comesfrom='" + company.Trim() + "' ;";
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow i in dt.Rows)
                {
                    var account = new Account();
                    account.id = int.Parse(i["iddata"].ToString());
                    account.rol = int.Parse(i["rol"].ToString());
                    account.rolstring = i["rolestring"].ToString();
                    account.rolObject = PMRol.GetOne(account.rol);
                    account.data.cellphone = i["cellphone"].ToString();
                    account.data.email = i["email"].ToString();
                    account.data.name = i["name"].ToString();
                    account.data.lastname = i["lastname"].ToString();
                    account.data.tcn = i["tcn"].ToString();
                    account.data.comesfrom = i["comesfrom"].ToString();
                    account.isactive = Convert.ToBoolean(i["activate"].ToString());
                    account.assignations = PMAccount.GetAssignations(account.id);
                    res.Add(account);
                }
            }
            return res;
        }
        public static List<Account> Get()
        {
            List<Account> res = new List<Account>();
            DataTable dt = new DataTable();
            string query = "SELECT ad.name, ad.lastname,ad.iddata, ad.email, ad.cellphone, ad.comesfrom,ad.tcn, ar.code as rolestring, ad.rol, ad.activate " +
                "FROM alfas_data ad " +
                "LEFT JOIN account_roles ar on ar.idrole = ad.rol;";
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow i in dt.Rows)
                {
                    var account = new Account();
                    account.id = int.Parse(i["iddata"].ToString());
                    account.rol = int.Parse(i["rol"].ToString());
                    account.rolstring = i["rolestring"].ToString();
                    account.rolObject = PMRol.GetOne(account.rol);
                    account.data.cellphone = i["cellphone"].ToString();
                    account.data.email = i["email"].ToString();
                    account.data.name = i["name"].ToString();
                    account.data.lastname = i["lastname"].ToString();
                    account.data.tcn = i["tcn"].ToString();
                    account.data.comesfrom = i["comesfrom"].ToString();
                    account.isactive = Convert.ToBoolean(i["activate"].ToString());
                    account.assignations = PMAccount.GetAssignations(account.id);
                    res.Add(account);
                }
            }
            return res;
        }
        public static Account GetOne(int id)
        {
            Account res = new Account();
            DataTable dt = new DataTable();
            string query = "SELECT ad.name, ad.lastname,ad.iddata, ad.email, ad.cellphone, ad.comesfrom,ad.tcn, ar.code as rolestring, ad.rol, ad.activate " +
                "FROM alfas_data ad " +
                "LEFT JOIN account_roles ar on ar.idrole = ad.rol " +
                "WHERE ad.iddata='" + id + "' ;";
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow i in dt.Rows)
                {
                    res.id = int.Parse(i["iddata"].ToString());
                    res.rol = int.Parse(i["rol"].ToString());
                    res.rolstring = i["rolestring"].ToString();
                    res.rolObject = PMRol.GetOne(res.rol);
                    res.data.cellphone = i["cellphone"].ToString();
                    res.data.email = i["email"].ToString();
                    res.data.name = i["name"].ToString();
                    res.data.lastname = i["lastname"].ToString();
                    res.data.tcn = i["tcn"].ToString();
                    res.data.comesfrom = i["comesfrom"].ToString();
                    res.isactive = Convert.ToBoolean(i["activate"].ToString());
                    res.assignations = PMAccount.GetAssignations(res.id);
                }
            }
            return res;
        }

    }
}