using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gamasis.ProjectManagement.Models.Objects
{
    public class Requirement : Incident
    {
        public Requirement()
        {
            files = new List<File>();
            programmers = new List<Programmer>();
            revisions = new List<Revision>();
            feedbacks = new List<Feedback>();
            id = 0;
            status = progress;
            idaccount = 0;
            priority = 0;
            type = 0;
            progress = 0;
            name = "";
            date = "";
            description = "";
            ubication = "";
            template = "";
        }
        [AllowHtml]
        public string template { set; get; } //Html text
        public int columns { set; get; } //Columns no
        public bool showhelper { set; get; }
        public int type { set; get; } // 1 Reporte, 2 Módulo, 3 Addon
        public int statussubtype { set; get; } //1 Diseño, 2 Codificación, 3 Pruebas, 4 Retro 
    }
}