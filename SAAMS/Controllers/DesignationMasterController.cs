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
    public class DesignationMasterController : Controller
    {
        // GET: DesignationMaster
        public SqlConnection connection;
       
        public ActionResult DesignationMaster()
        {
            List<DesignationMList_get_Result> designationMList_Get_Results = new List<DesignationMList_get_Result>();
            using (SecurityDBEntities context = new SecurityDBEntities())
            {
                designationMList_Get_Results = context.DesignationMList_get().ToList();
            }

            ViewBag.Details = designationMList_Get_Results;
            return View();

        }
        [HttpPost]
        public ActionResult Insert_UpdateDesignation(int hdnvalue, int DesignationId, string Designationname)
        {
            List<string> Response = new List<string>();
            using (SecurityDBEntities entities = new SecurityDBEntities())
            {
                Response = entities.DesignationMInsert_update_get(hdnvalue, DesignationId, Designationname, Convert.ToInt32(Session["Userid"])).ToList();
            }
            return Json(Response);
        }
        [HttpPost]
        public ActionResult DeleteDesignation(int id)
        {
            List<string> Response = new List<string>();
            using (SecurityDBEntities entities = new SecurityDBEntities())
            {
                Response = entities.DesignationMDelete_get(id, Convert.ToInt32(Session["Userid"])).ToList();
            }
            return Json(Response);
        }


        [HttpPost]
        public ActionResult DesignationMasterPdf()
        {
            string FileName = Path.Combine(Server.MapPath("~/Downloads/DesignationDetails/"));
            string filetodownload = "Designation Report " + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".pdf";


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
            Sqlcomrep.CommandText = "DesignationMList_get";

            SqlDataAdapter sqlda = new SqlDataAdapter(Sqlcomrep);
            SqlCon.Open();
            sqlda.Fill(dt);
            SqlCon.Close();



            //////////////////////////////////////////////////////////
            PdfPTable table0 = new PdfPTable(1);
            //table0.DefaultCell.Border = Rectangle.NO_BORDER;
            table0.WidthPercentage = 40;
            table0.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cell0;
            cell0 = new PdfPCell(new Phrase("DESIGNATION MASTER", F));
            cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell0.HorizontalAlignment = Element.ALIGN_CENTER;
            cell0.PaddingTop = 5;
            cell0.PaddingBottom = 7;
            // cell0.Rowspan = 3;
            cell0.BackgroundColor = new BaseColor(0, 119, 142);
            table0.AddCell(cell0);

            ////////////////////////////////////////////////////////////

            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 40;

            PdfPCell cell;
            cell = new PdfPCell(new Phrase("Sl.No", H));
            cell.Colspan = 1;
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("DESIGNATION NAME", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 4;
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

                // cell = new PdfPCell(new Phrase("aaaaaaaaaaaaaa", I));
                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("DesignationName").ToString(), I));
                cell.Colspan = 4;
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

            string fullPath = Path.Combine(Server.MapPath("~/Downloads/DesignationDetails/"), file);

            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/pdf", file);
        }
    }
}