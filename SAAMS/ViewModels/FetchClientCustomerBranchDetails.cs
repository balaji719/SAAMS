using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GMS.ViewModels
{
    public class FetchClientCustomerBranchDetails
    {
        public string showClient { get; set; }
        public string showCustomer { get; set; }
        public string showBranch { get; set; }
        public string showClustrer { get; set; }
        public string showSite { get; set; }
        public string showDevice { get; set; }
        public string showClientMandatory { get; set; }
        public string showCustomerMandatory { get; set; }
        public string showBranchMandatory { get; set; }
        public string showClustrerMandatory { get; set; }
        public string showSiteMandatory { get; set; }
        public string showDeviceMandatory { get; set; }
        public string CustomerName { get; set; }
        public int ClientId { get; set; }
        public int CustId { get; set; }
        public int BranchId { get; set; }
        public int ClustId { get; set; }
        public int SiteId { get; set; }
        public int Session_ClientId { get; set; }
        public string Session_ClientName { get; set; }
        public int Session_CustId { get; set; }
        public string Session_CustName { get; set; }
        public int Session_BranchId { get; set; }
        public string Session_BranchName { get; set; }
        public int Session_ClustId { get; set; }
        public string Session_ClusterName { get; set; }
        public int Session_SiteId { get; set; }
        public string Session_SiteName { get; set; }
    }
}