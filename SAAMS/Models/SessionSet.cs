using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
namespace SAAMS.Models
{
    public class SessionSet
    {
        public void SeeionSetLogin (int UserId,string UserName,string Pwd,int ProfileId,string ProfileCde,string ProfileNme,string CustName,string ClientNme,string BranchNme,string ClusterNme,string SiteNme,int clientId,string CustId,string BranchId,string ClustrId,string SiteId)
        {


            HttpContext.Current.Session["UserId"] = UserId;
            HttpContext.Current.Session["UserName"] = UserName;
            HttpContext.Current.Session["Pwd"] =  Pwd;
            HttpContext.Current.Session["ProfileId"] =  ProfileId;
            HttpContext.Current.Session["ProfileCode"] = ProfileCde;
            HttpContext.Current.Session["ProfileName"] = ProfileNme;
            HttpContext.Current.Session["CustMapping"] =  CustName;
            HttpContext.Current.Session["ClientName"] = ClientNme;
            HttpContext.Current.Session["BranchMapping"] = BranchNme;
            HttpContext.Current.Session["ClusterMapping"] = ClusterNme;
            HttpContext.Current.Session["SiteMapping"] = SiteNme;
            HttpContext.Current.Session["ClientId"] = clientId;
            HttpContext.Current.Session["CustId"] =  CustId;
            HttpContext.Current.Session["BranchId"] =  BranchId;
            HttpContext.Current.Session["ClusterId"] =  ClustrId;
            HttpContext.Current.Session["SiteId"] =  SiteId;
            HttpContext.Current.Session["SessionId"] = UserName;

        }
    }
}