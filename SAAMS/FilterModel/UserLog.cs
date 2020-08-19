using MongoDB.Driver;
using SAAMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SAAMS.FilterModel
{
    public class UserLog : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
            if ((string)filterContext.HttpContext.Session.Contents["SessionId"] == "" || (string)filterContext.HttpContext.Session.Contents["SessionId"] == null || (int)filterContext.HttpContext.Session.Contents["UserId"] == 0)
            {
                var context = new RequestContext(
                                  new HttpContextWrapper(System.Web.HttpContext.Current),
                                  new RouteData());
                var urlHelper = new UrlHelper(context);
                var url = urlHelper.Action("login", "login");
                //using (G4SOMSFEntities entities = new G4SOMSFEntities())
                //{

                //    entities.LastLogin_Set(Convert.ToInt32(GetUserId.Userid));
                //}
                filterContext.HttpContext.Session["SessioExp"] = "Session Expired Please Login Again....!";
                string a = Convert.ToString(filterContext.HttpContext.Session["SessioExp"]);
                System.Web.HttpContext.Current.Response.Redirect(url);
            }
            else
            {
                string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                string actionName = string.Concat(filterContext.ActionDescriptor.ActionName);

                UserLogDetails Logdata = new UserLogDetails();

                Logdata.SessionId = (string)filterContext.HttpContext.Session.Contents["SessionId"];
                Logdata.IpAddress = (string)filterContext.HttpContext.Session.Contents["IpAddress"];
                Logdata.UserID = (int)filterContext.HttpContext.Session.Contents["UserId"];
                Logdata.ActionName = controller + "/" + actionName;
                Logdata.EntryDate = DateTime.Now;

                string constr = ConfigurationManager.AppSettings["connectionString"];
                var Client = new MongoClient(constr);
                var DB = Client.GetDatabase("UserLogs");
                var collection = DB.GetCollection<UserLogDetails>("UsrLog");
                collection.InsertOneAsync(Logdata);
            }

        }
    }
}