using SAAMS.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAAMS.EntityFrameWork;
using SAAMS.Models;

namespace SAAMS.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
           
            return View();
        }

        public ActionResult LoginIn(LoginINM login)
        {
             if (login !=null)
             {
                AES256 aES256Enc = new AES256();
                login.Pwd = aES256Enc.EncryptString(login.Pwd.Trim(), ConfigurationManager.AppSettings["AESkey1"].ToString());

                using (SecurityDBEntities gMSEntities = new SecurityDBEntities())
                {
                    List<logindetails_get_Result> dbresult = gMSEntities.logindetails_get(login.UserName, login.Pwd).ToList();
                    if (dbresult.Count > 0)
                    {
                        foreach (logindetails_get_Result result in dbresult)
                        {

                          
                            SessionSet session = new SessionSet();
                            session.SeeionSetLogin(result.UserId, result.UserName, result.Pwd, result.ProfileId, result.ProfileCode, result.ProfileName, result.CustName, result.ClientName, result.BranchName, result.ClustrNme, result.SiteName, result.ClientId, result.CustId, result.BranchId, result.ClustrId, result.SiteId);
                            
                        }
                        //List<Menudetails_get_Result> menuresult = new List<Menudetails_get_Result>();
                        //var menudb = gMSEntities.Menudetails_get((int)Session["ProfileId"], Session["GMS"].ToString(), Session["EPS"].ToString(), Session["OMS"].ToString());
                        //foreach (Menudetails_get_Result mrs in menudb)
                        //{
                        //    menuresult.Add(mrs);
                        //}
                        //Session["MenuMaster"] = menuresult;
                        //Session["SessionId"] = Session.SessionID;

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {

                        TempData["StatusMsg"] = "Invalid User and Password.";
                        return RedirectToAction("Login", "Login");
                    }
                }

            }
            else
            {
                TempData["StatusMsg"] = "UserName and Password cannot be Empty";
            }
            return RedirectToAction("Login", "Login");

        }
    }
}