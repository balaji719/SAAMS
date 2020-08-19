using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SAAMS.EntityFrameWork;
using SAAMS.FilterModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAAMS.Controllers
{
    [ExceptionHandler]
    [UserLog]
    public class BranchMasterController : Controller
    {
        // GET: BranchMaster
        public ActionResult BranchMaster()
        {
            List<BranchMList_get_Result> BranchList = new List<BranchMList_get_Result>();
            using (SecurityDBEntities entities = new SecurityDBEntities())
            {
                BranchList = entities.BranchMList_get(Convert.ToInt32(Session["ProfileId"]), Convert.ToInt32(Session["UserId"])).ToList();
            }
            ViewBag.load = BranchList;
            return View();
           
        }
        [HttpPost]
        public ActionResult Insert_UpdateBranch(string Insert_Update, int hdnBranchId, int CustomerId, string BranchName, string BranchAddress, string ContactPerson, string BranchContactMobile, string BranchContactEmailId)
        {
            List<string> Response = new List<string>();
            using (SecurityDBEntities entites = new SecurityDBEntities())
            {
                Response = entites.BranchMInsert_Update_set(Convert.ToInt32(Insert_Update),hdnBranchId, CustomerId, BranchName, BranchAddress, ContactPerson, BranchContactMobile, BranchContactEmailId, Convert.ToInt32(Session["Userid"])).ToList();
            }

            return Json(Response);
        }
        [HttpPost]
        public ActionResult DeleteBranch(int BranchId)
        {
            List<string> Response = new List<string>();
            using (SecurityDBEntities entites = new SecurityDBEntities())
            {
                Response = entites.BranchMDelete_get(BranchId, Convert.ToInt32(Session["Userid"])).ToList();
            }


            return Json(Response);
        }
        [HttpPost]
        public ActionResult WriteExcel()
        {

            string filetodownload = string.Empty;
            string response = "NoData";
            try
            {

                string startPath = Server.MapPath("~/Downloads/BranchDetails/");
                if (Directory.Exists(startPath))
                {
                    foreach (string _file in Directory.GetFiles(startPath))
                    {
                        System.IO.File.Delete(_file);
                    }
                }




                filetodownload = "Branch Report " + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
                DataTable dt = new DataTable();
                SqlConnection SqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["MSSQL_ConnectionString"].ConnectionString);
                SqlCommand Sqlcomrep = new SqlCommand();
                Sqlcomrep.Connection = SqlCon;
                Sqlcomrep.CommandType = CommandType.StoredProcedure;
                Sqlcomrep.CommandTimeout = 500;
                Sqlcomrep.CommandText = "BranchMList_get";
                Sqlcomrep.Parameters.AddWithValue("@ProfileId", Convert.ToInt32(Session["ProfileId"]));
                Sqlcomrep.Parameters.AddWithValue("@UserId", Convert.ToInt32(Session["UserId"]));
                SqlDataAdapter sqlda = new SqlDataAdapter(Sqlcomrep);
                SqlCon.Open();
                sqlda.Fill(dt);
                SqlCon.Close();

                using (XLWorkbook wb = new XLWorkbook())
                {

                    wb.Worksheets.Add(dt, "Branch Details");
                    wb.SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/Downloads/BranchDetails/" + filetodownload));
                }



                response = "Reports Successfully Created";

            }
            catch (Exception ex)
            {
                response = "Error in Creating Reports";
                string logfilename = "ErrorLog" + DateTime.Now.Date.ToString("ddMMyyyy") + ".txt";
                string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                message += Environment.NewLine;
                message += string.Format("TargetSite: {0}", ex.ToString());
                message += Environment.NewLine;
                message += "-----------------------------------------------------------";
                message += Environment.NewLine;
                string path = System.Web.HttpContext.Current.Server.MapPath("~/Log/" + logfilename);
                //string path = System.IO.Directory.GetParent(Environment.CurrentDirectory).ToString();
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(message);
                    writer.Close();
                }
            }

            return Json(filetodownload);
        }

        public ActionResult Download(string file)
        {
            //get the temp folder and file path in server

            string fullPath = Path.Combine(Server.MapPath("~/Downloads/BranchDetails/"), file);

            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/vnd.ms-excel", file);
        }
        [HttpPost]
        public ActionResult BranchMasterPdf()
        {
            string FileName = Path.Combine(Server.MapPath("~/Downloads/BranchDetails/"));
            string filetodownload = "Branch Report " + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".pdf";


            string pathString = System.IO.Path.Combine(FileName, filetodownload);
            var file = System.IO.File.Create(pathString);
            file.Close();
            FileStream stream = new FileStream(pathString, FileMode.OpenOrCreate);
            Document doc = new Document(PageSize.A4.Rotate(), 40f, 40f, 20f, 20f);
            PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);

            var F = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
            var H = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
            var P = new BaseColor(102, 51, 153);
            var I = FontFactory.GetFont(FontFactory.HELVETICA, 9, P);



            DataTable dt = new DataTable();
            SqlConnection SqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["MSSQL_ConnectionString"].ConnectionString);
            SqlCommand Sqlcomrep = new SqlCommand();
            Sqlcomrep.Connection = SqlCon;
            Sqlcomrep.CommandType = CommandType.StoredProcedure;
            Sqlcomrep.CommandTimeout = 500;
            Sqlcomrep.CommandText = "BranchMList_get";
            Sqlcomrep.Parameters.AddWithValue("@ProfileId", Convert.ToInt32(Session["ProfileId"]));
            Sqlcomrep.Parameters.AddWithValue("@UserId", Convert.ToInt32(Session["UserId"]));
            SqlDataAdapter sqlda = new SqlDataAdapter(Sqlcomrep);
            SqlCon.Open();
            sqlda.Fill(dt);
            SqlCon.Close();



            //////////////////////////////////////////////////////////
            PdfPTable table0 = new PdfPTable(1);
            //table0.DefaultCell.Border = Rectangle.NO_BORDER;
            table0.WidthPercentage = 100;
            table0.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cell0;
            cell0 = new PdfPCell(new Phrase("BRANCH MASTER", F));
            cell0.HorizontalAlignment = Element.ALIGN_CENTER;
            cell0.PaddingTop = 5;
            cell0.PaddingBottom = 7;
            // cell0.Rowspan = 3;
            cell0.BackgroundColor = new BaseColor(0, 119, 142);
            table0.AddCell(cell0);

            ////////////////////////////////////////////////////////////

            PdfPTable table = new PdfPTable(21);
            table.WidthPercentage = 100;

            PdfPCell cell;
            cell = new PdfPCell(new Phrase("Sl.No", H));
            cell.Colspan = 1;
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("CLIENTNAME", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 2;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("CUSTNAME", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 2;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("BRANCH NAME", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 3;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("CITY", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 2;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);


            cell = new PdfPCell(new Phrase("CONTACT PERSON", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 3;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("ADDRESS", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 2;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("CONTACT MOBILE", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 3;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("CONTACT MAILID", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 3;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            ///////////////////////////////
            int slno = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                slno = slno + 1;

                cell = new PdfPCell(new Phrase(slno.ToString(), I));
                cell.Colspan = 1;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("ClientName").ToString(), I));
                cell.Colspan = 2;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("CustName").ToString(), I));
                cell.Colspan = 2;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("BranchName").ToString(), I));
                cell.Colspan = 3;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("ClientName").ToString(), I));//////
                cell.Colspan = 2;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("ContactName").ToString(), I));
                cell.Colspan = 3;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("Address").ToString(), I));
                cell.Colspan = 2;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);


                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("ContactMobile").ToString(), I));
                cell.Colspan = 3;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("ContactEmailId").ToString(), I));
                cell.Colspan = 3;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
            }
            ///////////////////////////////
            //cell = new PdfPCell(new Phrase("1234", I));
            //cell.Colspan = 1;
            //cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("aaaaaaaaaaaaaa", I));
            //cell.Colspan = 2;
            //cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("qqqqqqqq", I));
            //cell.Colspan = 2;
            //cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("1234", I));
            //cell.Colspan = 3;
            //cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("aaaaaaaaaaaa", I));
            //cell.Colspan = 2;
            //cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("1234", I));
            //cell.Colspan = 3;
            //cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("aaaaaaaaaaaaaa", I));
            //cell.Colspan = 2;
            //cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //table.AddCell(cell);

            ////cell = new PdfPCell(new Phrase("qqqqqqqq", I));
            ////cell.Colspan = 2;
            ////cell.HorizontalAlignment = Element.ALIGN_CENTER;
            ////table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("1234", I));
            //cell.Colspan = 3;
            //cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("1234", I));
            //cell.Colspan = 3;
            //cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //table.AddCell(cell);
            // table0.SpacingBefore = 10f;
            table0.SpacingAfter = 12.5f;
            //  table.SpacingBefore = 10f;
            // table.SpacingAfter = 12.5f;


            doc.Open();
            doc.Add(table0);
            doc.Add(table);
            doc.Close();

            return Json(filetodownload);
        }


        public ActionResult Downloadpdf(string file)
        {
            //get the temp folder and file path in server

            string fullPath = Path.Combine(Server.MapPath("~/Downloads/BranchDetails/"), file);

            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/pdf", file);
        }
    }
}