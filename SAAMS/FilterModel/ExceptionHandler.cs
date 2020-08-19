using Logger;
using MongoDB.Driver;
using SAAMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAAMS.FilterModel
{
    public class ExceptionHandler : FilterAttribute, IExceptionFilter
    {
        private ILogger logger = LoggerFactory.getLogger();

        public void OnException(ExceptionContext filterContext)
        {
            string controller = filterContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();

            if (!filterContext.ExceptionHandled && (string)filterContext.HttpContext.Session.Contents["SessionId"] != null)
            {

                //ActionfilterEntity ActionFE = new ActionfilterEntity();

                ExceptionLogger logger = new ExceptionLogger();
                logger.SessionId = (string)filterContext.HttpContext.Session.Contents["SessionId"];
                logger.ErrorMsg = filterContext.Exception.Message;
                logger.ActionName = controller + "/" + actionName;
                logger.UserId = (int)filterContext.HttpContext.Session.Contents["UserId"];
                logger.EntryDate = DateTime.Now;



                string constr = ConfigurationManager.AppSettings["connectionString"];
                var Client = new MongoClient(constr);
                var DB = Client.GetDatabase("Exceptionogs");
                var collection = DB.GetCollection<ExceptionLogger>("Exceptionogs");
                collection.InsertOneAsync(logger);

            }

            logger.LogException("", Severity.ProcessingError, controller, actionName, filterContext.Exception);
        }
    }
}