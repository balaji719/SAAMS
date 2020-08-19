using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAAMS.Models
{
    public class LoginINM
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter User Name")]
        [StringLength(50, ErrorMessage = "message length exceeded", MinimumLength = 1)]
        public string UserName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Password")]
        public string Pwd { get; set; }
    }
}