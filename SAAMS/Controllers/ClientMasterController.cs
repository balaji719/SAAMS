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
    public class ClientMasterController : Controller
    {
        // GET: ClientMaster
        public ActionResult ClientMaster()
        {
           
            List<ClientMList_get_Result> client_Results = new List<ClientMList_get_Result>();
            using (SecurityDBEntities context = new SecurityDBEntities())
            {
                client_Results = context.ClientMList_get(Convert.ToInt32(Session["ClientId"])).ToList();
            }

            ViewBag.details = client_Results;
            return View();
        }

        [HttpPost]
        public ActionResult Insert_UpdateClient(int hdnvalue, int ClientId, string ClientName, string ClientPhNo, string ClientAddr, string AuthPersonName, string AuthPersonNo, string AuthPersonMailid)
        {
            List<string> Response = new List<string>();
            using (SecurityDBEntities entities = new SecurityDBEntities())
            {
                Response = entities.ClientMInsert_update_get(hdnvalue, ClientId, ClientName, ClientPhNo, ClientAddr, AuthPersonName, AuthPersonNo, AuthPersonMailid, Convert.ToInt32(Session["Userid"])).ToList();
            }


            return Json(Response);
        }
        [HttpPost]
        public ActionResult DeleteClient(int id)
        {
            List<string> Response = new List<string>();
            using (SecurityDBEntities entities = new SecurityDBEntities())
            {
                Response = entities.ClientMDelete_get(id, Convert.ToInt32(Session["Userid"])).ToList();
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

                string startPath = Server.MapPath("~/Downloads/ClientDetails/");
                if (Directory.Exists(startPath))
                {
                    foreach (string _file in Directory.GetFiles(startPath))
                    {
                        System.IO.File.Delete(_file);
                    }
                }




                filetodownload = "Client Report " + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
                DataTable dt = new DataTable();
                SqlConnection SqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["MSSQL_ConnectionString"].ConnectionString);
                SqlCommand Sqlcomrep = new SqlCommand();
                Sqlcomrep.Connection = SqlCon;
                Sqlcomrep.CommandType = CommandType.StoredProcedure;
                Sqlcomrep.CommandTimeout = 500;
                Sqlcomrep.CommandText = "ClientMList_get";
                Sqlcomrep.Parameters.AddWithValue("@ClientId", Convert.ToInt32(Session["ClientId"]));

                SqlDataAdapter sqlda = new SqlDataAdapter(Sqlcomrep);
                SqlCon.Open();
                sqlda.Fill(dt);
                SqlCon.Close();

                using (XLWorkbook wb = new XLWorkbook())
                {

                    wb.Worksheets.Add(dt, "Clients");
                    wb.SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/Downloads/ClientDetails/" + filetodownload));
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

            string fullPath = Path.Combine(Server.MapPath("~/Downloads/ClientDetails/"), file);

            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/vnd.ms-excel", file);
        }


        [HttpPost]
        public ActionResult ClientMasterPdf()
        {

            string FileName = Path.Combine(Server.MapPath("~/Downloads/ClientDetails/"));
            string filetodownload = "Client Report " + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".pdf";


            string pathString = System.IO.Path.Combine(FileName, filetodownload);
            var file = System.IO.File.Create(pathString);
            file.Close();

            FileStream stream = new FileStream(pathString, FileMode.OpenOrCreate);
            Document doc = new Document(PageSize.A4, 40f, 40f, 20f, 20f);
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
            Sqlcomrep.CommandText = "ClientMList_get";
            Sqlcomrep.Parameters.AddWithValue("@ClientId", Convert.ToInt32(Session["ClientId"]));

            SqlDataAdapter sqlda = new SqlDataAdapter(Sqlcomrep);
            SqlCon.Open();
            sqlda.Fill(dt);
            SqlCon.Close();



            PdfPTable table0 = new PdfPTable(1);
            //table0.DefaultCell.Border = Rectangle.NO_BORDER;
            table0.WidthPercentage = 100;
            table0.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cell0;
            cell0 = new PdfPCell(new Phrase("CLIENT  MASTER", F));
            cell0.HorizontalAlignment = Element.ALIGN_CENTER;
            cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell0.PaddingTop = 5;
            cell0.PaddingBottom = 7;
            // cell0.Rowspan = 3;
            cell0.BackgroundColor = new BaseColor(0, 119, 142);
            table0.AddCell(cell0);

            ////////////////////////////////////////////////////////////

            PdfPTable table = new PdfPTable(12);
            table.WidthPercentage = 100;

            PdfPCell cell;
            cell = new PdfPCell(new Phrase("Sl.No", H));
            cell.Colspan = 1;
            cell.PaddingBottom = 5;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("CLIENT NAME", H));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = 3;
            cell.PaddingBottom = 5;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("CONTACT PERSON", H));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = 3;
            cell.PaddingBottom = 5;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("CONTACT PHNO", H));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = 2;
            cell.PaddingBottom = 5;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("CONTACT MAILID", H));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = 3;
            cell.PaddingBottom = 5;
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
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase((dt.Rows[i].Field<string>("ClientName")).ToString(), I));
                cell.Colspan = 3;
                cell.PaddingBottom = 5;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase((dt.Rows[i].Field<string>("AuthPersonName")).ToString(), I));
                cell.Colspan = 3;
                cell.PaddingBottom = 5;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase((dt.Rows[i].Field<string>("AuthPersonContactNo")).ToString(), I));
                cell.Colspan = 2;
                cell.PaddingBottom = 5;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase((dt.Rows[i].Field<string>("AuthPersonEmailId")).ToString(), I));
                cell.Colspan = 3;
                cell.PaddingBottom = 5;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);

            }
            table0.SpacingBefore = 10f;
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

            string fullPath = Path.Combine(Server.MapPath("~/Downloads/ClientDetails/"), file);

            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/pdf", file);
        }

    }
}