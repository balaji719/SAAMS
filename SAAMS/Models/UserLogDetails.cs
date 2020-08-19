using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAAMS.Models
{
    public class UserLogDetails
    {
        public string SessionId { get; set; }
        public string IpAddress { get; set; }
        public int UserID { get; set; }
        public string ActionName { get; set; }
        public DateTime EntryDate { get; set; }
    }
}