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
    public class AlaveMethods
    {
        //public static MainPerson existin(Account account)
        //{
        //    MainPerson person = new MainPerson();
        //    string query = "SELECT * FROM cat_personas WHERE SIS_USER='" + account.username + "' AND SIS_PASSWORD='" + account.password + "';";
        //    DataTable dt = SQL_Queries.Query_Get(query, ConnectionHelper.getConnString());
        //    if (dt.Rows.Count > 0)
        //    {
        //        DataRow Item = dt.Rows[0];
        //        try { person.details.Active = bool.Parse(Item["Authorized_Person"].ToString()); } catch { }
        //        try { person.details.Avatar = (byte[])Item["FOTO"]; } catch { }
        //        try { person.details.Email = Item["MAIL"].ToString(); } catch { }
        //        try { person.alfas.CellPhone = Item["celular"].ToString(); } catch { }
        //        try { person.alfas.FirstName = Item["PRIMER_NOMBRE"].ToString(); } catch { }
        //        try { person.alfas.SecondName = Item["SEGUNDO_NOMBRE"].ToString(); } catch { }
        //        try { person.alfas.LastName = Item["PRIMER_APELLIDO"].ToString(); } catch { }
        //        try { person.alfas.SecondLastName = Item["SEGUNDO_APELLIDO"].ToString(); } catch { }
        //        try { person.alfas.Phone = Item["telefono"].ToString(); } catch { }
        //        try { person.details.Id = int.Parse(Item["ID"].ToString()); } catch { }
        //        try { person.details.Profile = Item["PERFIL_NIVEL"].ToString(); } catch { }
        //        try { person.details.Username = Item["SIS_USER"].ToString(); } catch { }
        //    }
        //    return person;
        //}
    }
}