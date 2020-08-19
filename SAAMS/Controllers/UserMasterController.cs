using SAAMS.FilterModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAAMS.Controllers
{
    [ExceptionHandler]
    [UserLog]
    public class UserMasterController : Controller
    {
        // GET: UserMaster
        public ActionResult UserMaster()
        {
            return View();
        }
    }
}