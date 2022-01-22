using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gamasis.ProjectManagement.Models.Objects
{
    public class Incident
    {
        public int id { set; get; }
        public int idaccount { set; get; }
        public string accountName { set; get; }
        public string accountMail { set; get; }
        public string accountPhone { set; get; }
        public int priority { set; get; }
        public int status { set; get; }
        public int progress { set; get; }
        public string name { set; get; }
        public string date { set; get; }
        public string concludedby { set; get; }
        public int concludedbyid { set; get; }
        public string concludeddate { set; get; }
        public string description { set; get; }
        public string ubication { set; get; }
        public List<File> files { set; get; }
        public List<Programmer> programmers { set; get; }
        public List<Revision> revisions { set; get; }
        public List<Feedback> feedbacks { set; get; }
        public List<Comment> comments { set; get; }
        public bool checkifexistprogrammer(int idpro)
        {
            foreach(Programmer p in programmers)
            {
                if (p.idpro == idpro)
                    return true;
            }
            return false;
        }
        public Incident()
        {
            files = new List<File>();
            programmers = new List<Programmer>();
            revisions = new List<Revision>();
            feedbacks = new List<Feedback>();
            comments = new List<Comment>();
            id = 0;
            status = progress;
            idaccount = 0;
            priority = 0;
            progress = 0;
            name = "";
            date = "";
            description = "";
            ubication = "";
        }
       
    }
}