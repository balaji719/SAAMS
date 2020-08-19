using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAAMS.EntityFrameWork;

namespace SAAMS.Controllers
{
    public class AllLevelDDController : Controller
    {
        //MainGmsEntity entities = new MainGmsEntity();
        //GET: DropdownPartial
        //[HttpPost]
        public ActionResult DDClient()
        {
            List<DDClients_get_Result> ClientRes = new List<DDClients_get_Result>();          
            if (Session["ProfileId"] != null)
            {
                if (Convert.ToString(Session["ProfileId"]) == "1" || Convert.ToString(Session["ProfileId"]) == "2")
                {
                    using (SecurityDBEntities entities = new SecurityDBEntities())
                    {
                        ClientRes = entities.DDClients_get(0).ToList();
                    }
                }
                else
                {
                    DDClients_get_Result dDClients = new DDClients_get_Result();
                    dDClients.ClientName =Convert.ToString(Session["ClientName"]);
                    dDClients.ClientId = Convert.ToInt32(Session["ClientId"]);
                    ClientRes.Add(dDClients);
                }
                

            }

            return Json(ClientRes);
        }
        [HttpPost]
        public ActionResult DDCustomer(int ClientId)
        {
            List<DDCustomers_get_Result> dDCustomers = new List<DDCustomers_get_Result>();

            if (Session["ProfileId"] != null)
            {
                if (Convert.ToString(Session["ProfileId"]) == "1" || Convert.ToString(Session["ProfileId"]) == "2")
                {
                    using (SecurityDBEntities entities = new SecurityDBEntities())
                    {
                        dDCustomers = entities.DDCustomers_get(ClientId).ToList();
                    }
                }
                else
                {
                    string[] CustName = Convert.ToString(Session["CustMapping"]).Split(new string[] { "||" }, StringSplitOptions.None);
                    string[] CustId= Convert.ToString(Session["CustId"]).Split(new string[] { "||" }, StringSplitOptions.None);
                    DDCustomers_get_Result dDCustomers_Get_ = new DDCustomers_get_Result();
                   for(int i=0;i<CustName.Length;i++)
                   {
                        dDCustomers_Get_.CustName = CustName[i].ToString();                    
                   }
                    for (int i = 0; i < CustId.Length; i++)
                    {
                        dDCustomers_Get_.CustId =Convert.ToInt32(CustId[i]);
                    }
                    dDCustomers.Add(dDCustomers_Get_);
                }
            }
                return Json(dDCustomers);
        }

        [HttpPost]
        public ActionResult DropdownBranch(int CustId)
        {
            List<DDBranch_get_Result> dDBranch_s = new List<DDBranch_get_Result>();

            if (Session["ProfileId"] != null)
            {
                if (Convert.ToString(Session["ProfileId"]) == "1" || Convert.ToString(Session["ProfileId"]) == "2")
                {
                    using (SecurityDBEntities entities = new SecurityDBEntities())
                    {
                        dDBranch_s = entities.DDBranch_get(CustId).ToList();
                    }
                }
                else
                {
                    string[] BranchName = Convert.ToString(Session["BranchMapping"]).Split(new string[] { "||" }, StringSplitOptions.None);
                    string[] BranchId = Convert.ToString(Session["BranchId"]).Split(new string[] { "||" }, StringSplitOptions.None);
                    DDBranch_get_Result branch_Get_Result = new DDBranch_get_Result();
                    for (int i = 0; i < BranchName.Length; i++)
                    {
                        branch_Get_Result.BranchName = BranchName[i].ToString();
                    }
                    for (int i = 0; i < BranchId.Length; i++)
                    {
                        branch_Get_Result.BranchId = Convert.ToInt32(BranchId[i]);
                    }
                    dDBranch_s.Add(branch_Get_Result);
                }
            }
            return Json(dDBranch_s);
        }


        [HttpPost]
        public ActionResult DropdownCluster(int BranchId)
        {
            List<DDClusters_get_Result> dDClusters = new List<DDClusters_get_Result>();

            if (Session["ProfileId"] != null)
            {
                if (Convert.ToString(Session["ProfileId"]) == "1" || Convert.ToString(Session["ProfileId"]) == "2")
                {
                    using (SecurityDBEntities entities = new SecurityDBEntities())
                    {
                        dDClusters = entities.DDClusters_get(BranchId).ToList();
                    }
                }
                else
                {
                    string[] ClusterName = Convert.ToString(Session["ClusterMapping"]).Split(new string[] { "||" }, StringSplitOptions.None);
                    string[] ClusterId = Convert.ToString(Session["ClusterId"]).Split(new string[] { "||" }, StringSplitOptions.None);
                    DDClusters_get_Result dDClusters_ = new DDClusters_get_Result();
                    for (int i = 0; i < ClusterName.Length; i++)
                    {
                        dDClusters_.ClusterName = ClusterName[i].ToString();
                    }
                    for (int i = 0; i < ClusterId.Length; i++)
                    {
                        dDClusters_.ClusterId = Convert.ToInt32(ClusterId[i]);
                    }
                    dDClusters.Add(dDClusters_);
                }
            }
            return Json(dDClusters);
        }
        [HttpPost]
        public ActionResult DropdownSite(int ClusterId)
        {
            List<DDSites_get_Result> dDSites_Get_Results = new List<DDSites_get_Result>();

            if (Session["ProfileId"] != null)
            {
                if (Convert.ToString(Session["ProfileId"]) == "1" || Convert.ToString(Session["ProfileId"]) == "2")
                {
                    using (SecurityDBEntities entities = new SecurityDBEntities())
                    {
                        dDSites_Get_Results = entities.DDSites_get(ClusterId).ToList();
                    }
                }
                else
                {
                    string[] SiteName = Convert.ToString(Session["SiteMapping"]).Split(new string[] { "||" }, StringSplitOptions.None);
                    string[] SiteId = Convert.ToString(Session["SiteId"]).Split(new string[] { "||" }, StringSplitOptions.None);
                    DDSites_get_Result dDSites = new DDSites_get_Result();
                    for (int i = 0; i < SiteName.Length; i++)
                    {
                        dDSites.SiteName = SiteName[i].ToString();
                    }
                    for (int i = 0; i < SiteId.Length; i++)
                    {
                        dDSites.SiteId = Convert.ToInt32(SiteId[i]);
                    }
                    dDSites_Get_Results.Add(dDSites);
                }
            }
            return Json(dDSites_Get_Results);
        }
        // [HttpPost]
        // public ActionResult DropdownEmployee(int SiteId)
        // {
        //     List<DDEmployees_get_Result> EmployessResult = new List<DDEmployees_get_Result>();
        //     using (MainGmsEntity entities = new MainGmsEntity())
        //     {
        //         EmployessResult = entities.DDEmployees_get(SiteId).ToList();
        //     }

        //     return Json(EmployessResult);
        // }
        // [HttpPost]
        // public ActionResult DDClient()
        // {
        //     List<DDClients_get_Result> ClientResult = new List<DDClients_get_Result>();
        //     using (SecurityDBEntities entities = new SecurityDBEntities())
        //     {
        //         ClientResult = entities.DDClients_get(0).ToList();
        //     }

        //     return Json(ClientResult);
        // }
        [HttpPost]
        public ActionResult CountryGet()
        {
            List<DDCountry_Get_Result> dDCountry_s = new List<DDCountry_Get_Result>();
            using(SecurityDBEntities entities=new SecurityDBEntities())
            {
                dDCountry_s = entities.DDCountry_Get().ToList();
            }           
             return Json(dDCountry_s);
        }
        [HttpPost]
        public ActionResult StateDDget(int CountryId)
        {
            List<DDState_Get_Result>dDState_Gets = new List<DDState_Get_Result>();
            using (SecurityDBEntities entities = new SecurityDBEntities())
            {
                dDState_Gets = entities.DDState_Get(CountryId).ToList();
            }
            return Json(dDState_Gets);
        }

        [HttpPost]
        public ActionResult CityDDGet(int StateId)
        {
            List<DDCity_Get_Result> dDCity_s = new List<DDCity_Get_Result>();
            using(SecurityDBEntities entities=new SecurityDBEntities())
            {
                dDCity_s = entities.DDCity_Get(StateId).ToList();
            }
            return Json(dDCity_s);
        }
    }
}