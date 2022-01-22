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
    public class PMLogin
    {
        public static Account signin(Account account)
        {
            DataTable dt = new DataTable();
            string query = "SELECT ad.name, ad.lastname,a.idaccount,a.username, ad.email, ad.cellphone, ad.comesfrom,ad.tcn, ar.code as rolestring, a.idrole, a.status " +
                "FROM account a " +
                "LEFT JOIN alfas_data ad on ad.idaccount= a.idaccount " +
                "LEFT JOIN account_roles ar on ar.idrole = a.idrole " +
                "WHERE a.username='" + account.username.Trim() + "' AND aes_decrypt(a.password,'gpm.encrypt')='" + account.password.Trim() + "' ;";
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al iniciar sessión: {0} ", ex.Message)); }
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow i in dt.Rows)
                {
                    account.id = int.Parse(i["idaccount"].ToString());
                    account.rol = int.Parse(i["idrole"].ToString());
                    account.rolstring = i["rolestring"].ToString();
                    account.data.cellphone = i["cellphone"].ToString();
                    account.data.email = i["email"].ToString();
                    account.data.name = i["name"].ToString();
                    account.data.lastname = i["lastname"].ToString();
                    account.data.tcn = i["tcn"].ToString();
                    account.data.comesfrom = i["comesfrom"].ToString();
                    account.isactive = (i["Status"].ToString() == "Active" ? true : false);
                }
            }
            return account;
        }
        //public static Account find(Account account, string type = "Alave")
        //{
        //    switch (type)
        //    {
        //        case "Alave":
        //            MainPerson a = AlaveMethods.existin(account);
        //            if (a.details.Id != 0)
        //            {
        //                account.data.idfromproject = a.details.Id;
        //                account.data.comesfrom = "ALAVE";
        //                account.data.name = a.alfas.FirstName + " " + a.alfas.SecondName;
        //                account.data.lastname = a.alfas.LastName + " " + a.alfas.SecondLastName;
        //                account.data.email = a.details.Email;
        //                account.data.cellphone = a.alfas.CellPhone;
        //                account.data.phone = a.alfas.Phone;
        //            }
        //            return account;
        //        case "":
        //            return new Account();
        //    }
        //    return account;
        //}
        public static int insertaccesslog(Account account, string platform = "WEB")
        {
            int res = 0;
            int platfominteger = 1;

            switch (platform)
            {
                case "WEB":
                    platfominteger = 1;
                    break;
                case "DESKTOP":
                    platfominteger = 2;
                    break;
                case "MOBILE":
                    platfominteger = 3;
                    break;
            }

            string query = "INSERT INTO `gpm`.`account_log` " +
                "(date,ip,accountid,platform,val,type) VALUES (" +
                //"sysdate()" + "," +
                "'" + account.sessionStartedAt + "'," +
                "'" + account.ip + "'," +
                "'" + account.id + "'," +
                "'" + platfominteger + "'," +
                "'Login'," +
                "" + 1 + ") ;";
            try { res = SQL_Queries.Query_Execute(query, ConnectionHelper.getConnString("gpmdb")); } catch (Exception ex) { Gamasis.Utils.GLog.Write(Url.GetLogsPath(), string.Format("Ocurrió un error al registar el log: {0} ", ex.Message)); }
            return res;
        }
        public static Alfas Exists(int idfromproject, string comesfrom)
        {
            Alfas res = new Alfas();
            DataTable dt = new DataTable();
            string query = string.Format("SELECT ad.activate,ad.idfromproject, ad.iddata, ad.cellphone, ad.phone, ad.comesfrom, ad.email, ad.name, ad.lastname, ad.tcn, ad.rol, (SELECT ar.description FROM account_roles ar where ar.idrole=ad.rol) as rolstring FROM gpm.alfas_data ad WHERE ad.comesfrom='{0}' AND ad.idfromproject='{1}';", comesfrom, idfromproject);
            try { dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString("gpmdb")); } catch { }
            foreach (DataRow dr in dt.Rows)
            {
                res.iddata = int.Parse(dr["iddata"].ToString());
                res.idfromproject = int.Parse(dr["idfromproject"].ToString());
                res.rol = int.Parse(dr["rol"].ToString());
                res.rolstring = dr["rolstring"].ToString();
                res.activate = Convert.ToBoolean(dr["activate"].ToString());
                res.comesfrom = dr["comesfrom"].ToString();
                res.cellphone = dr["cellphone"].ToString();
                res.email = dr["email"].ToString();
                res.name = dr["name"].ToString();
                res.lastname = dr["lastname"].ToString();
                res.phone = dr["phone"].ToString();
                res.tcn = dr["tcn"].ToString();
            }
            return res;
        }
    }
}