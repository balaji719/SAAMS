using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAAMS.Models
{
    public class ExceptionLogger
    {
        public int UserId { get; set; }
        public string SessionId { get; set; }
        public string ActionName { get; set; }
        public string ErrorMsg { get; set; }
        public System.DateTime EntryDate { get; set; }
    }
}