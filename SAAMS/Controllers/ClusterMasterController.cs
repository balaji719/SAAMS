﻿using ClosedXML.Excel;
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
    public class ClusterMasterController : Controller
    {
        // GET: ClusterMaster
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ClusterMaster()
        {
            List<ClusterMList_get_Result> cm = new List<ClusterMList_get_Result>();
            using (SecurityDBEntities entities = new SecurityDBEntities())
            {
                cm = entities.ClusterMList_get(Convert.ToInt32(Session["ProfileId"]), Convert.ToInt32(Session["UserId"])).ToList();

            }
            ViewBag.load = cm;
            return View();

        }
        [HttpPost]

        public ActionResult ClusterUpdate(string Insert_Update, int BranchId, int ClusterId, string ClusterName, string ClusterAddress, string ContactPerson, string ContactMobile, string ContactEmailId)
        {
            List<string> Response = new List<string>();
            using (SecurityDBEntities entites = new SecurityDBEntities())
            {
                Response = entites.ClusterMInsert_Update_set(Convert.ToInt32(Insert_Update), BranchId, ClusterId, ClusterName, ClusterAddress, ContactPerson, ContactMobile, ContactEmailId, Convert.ToInt32(Session["Userid"])).ToList();

            }

            return Json(Response);
        }
        [HttpPost]
        public ActionResult DeleteCluster(int ClusterId)
        {
            List<string> Response = new List<string>();
            using (SecurityDBEntities entites = new SecurityDBEntities())
            {
                Response = entites.ClusterMDelete_get(ClusterId, Convert.ToInt32(Session["Userid"])).ToList();
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

                string startPath = Server.MapPath("~/Downloads/ClusterDetails/");
                if (Directory.Exists(startPath))
                {
                    foreach (string _file in Directory.GetFiles(startPath))
                    {
                        System.IO.File.Delete(_file);
                    }
                }




                filetodownload = "Cluster Report " + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
                DataTable dt = new DataTable();
                SqlConnection SqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["MSSQL_ConnectionString"].ConnectionString);
                SqlCommand Sqlcomrep = new SqlCommand();
                Sqlcomrep.Connection = SqlCon;
                Sqlcomrep.CommandType = CommandType.StoredProcedure;
                Sqlcomrep.CommandTimeout = 500;
                Sqlcomrep.CommandText = "ClusterMList_get";
                Sqlcomrep.Parameters.AddWithValue("@ProfileId", Convert.ToInt32(Session["ProfileId"]));
                Sqlcomrep.Parameters.AddWithValue("@UserId", Convert.ToInt32(Session["UserId"]));
                SqlDataAdapter sqlda = new SqlDataAdapter(Sqlcomrep);
                SqlCon.Open();
                sqlda.Fill(dt);
                SqlCon.Close();

                using (XLWorkbook wb = new XLWorkbook())
                {

                    wb.Worksheets.Add(dt, "Cluster Details");
                    wb.SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/Downloads/ClusterDetails/" + filetodownload));
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

            string fullPath = Path.Combine(Server.MapPath("~/Downloads/ClusterDetails/"), file);

            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/vnd.ms-excel", file);
        }


        [HttpPost]
        public ActionResult ClustemerMasterPdf()
        {
            string FileName = Path.Combine(Server.MapPath("~/Downloads/ClusterDetails/"));
            string filetodownload = "Cluster Report " + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".pdf";


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
            Sqlcomrep.CommandText = "ClusterMList_get";
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
            cell0 = new PdfPCell(new Phrase("CLUSTER MASTER", F));
            cell0.HorizontalAlignment = Element.ALIGN_CENTER;
            cell0.PaddingTop = 5;
            cell0.PaddingBottom = 7;
            // cell0.Rowspan = 3;
            cell0.BackgroundColor = new BaseColor(0, 119, 142);
            table0.AddCell(cell0);

            ////////////////////////////////////////////////////////////

            PdfPTable table = new PdfPTable(13);
            table.WidthPercentage = 100;

            PdfPCell cell;
            cell = new PdfPCell(new Phrase("Sl.No", H));
            cell.Colspan = 1;
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("CLIENT NAME", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 2;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("CUSTOMER NAME", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 2;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("BRANCH NAME	", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 2;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("CLUSTER NAME", H));
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
            cell.Colspan = 2;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("MOBILE NUMBER", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 2;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);



            ///////////////////////////////
            ///
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
                cell.Colspan = 2;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);


                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("ClusterName").ToString(), I));
                cell.Colspan = 2;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("ContactPerson").ToString(), I));
                cell.Colspan = 2;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("ContactMobile").ToString(), I));
                cell.Colspan = 2;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
            }




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

            string fullPath = Path.Combine(Server.MapPath("~/Downloads/ClusterDetails/"), file);

            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/pdf", file);
        }




    }
    

}