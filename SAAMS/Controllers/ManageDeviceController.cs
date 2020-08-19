﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SAAMS.EntityFrameWork;
using SAAMS.FilterModel;

namespace SAAMS.Controllers
{
    [ExceptionHandler]
    [UserLog]
    public class ManageDeviceController : Controller
    {
        // GET: ManageDevice
        public ActionResult ManageDevice()
        {
            List<DeviceMList_get_Result> DeviceList = new List<DeviceMList_get_Result>();
            using (SecurityDBEntities entity = new SecurityDBEntities())
            {
                DeviceList = entity.DeviceMList_get().ToList();
            }
            ViewBag.deviceviewall = DeviceList;
            return View();
        }
        [HttpPost]
        public ActionResult EditDevice(string DeviceId)
        {
            List<DeviceMEdit_get_Result> DeviceResult = new List<DeviceMEdit_get_Result>();
            using (SecurityDBEntities entity = new SecurityDBEntities())
            {
                DeviceResult = entity.DeviceMEdit_get(Convert.ToInt32(DeviceId)).ToList();
            }
            return Json(DeviceResult);
           
        }
        [HttpPost]
        public ActionResult InsertUpdateDevice(int Insert_Update,int hdnDeviceId, int ClusterId, int ClientId, string SiteName, string DeviceIMEI, string DevicePhone, string DeviceCarrier, string DeviceType, string DeviceState, string EmailId, string SuperVisorPassword, string Superuserpassword, string WarrantyStartDate, string WarrantyEndDate, string DeviceAlertSMS1, string DeviceAlertSMS2, string DeviceAlertEmail1, string DeviceAlertEmail2, string Location)
        {
            List<string> Response = new List<string>();
            using (SecurityDBEntities entity = new SecurityDBEntities())
            {
                Response = entity.DeviceMInsert_Update_set(Insert_Update, hdnDeviceId, ClientId, ClusterId, SiteName, DeviceIMEI, DevicePhone, DeviceCarrier, DeviceType, DeviceState, SuperVisorPassword, Superuserpassword, EmailId, WarrantyStartDate, WarrantyEndDate, DeviceAlertSMS1, DeviceAlertSMS2, DeviceAlertEmail1, DeviceAlertEmail2, Location, Convert.ToInt32(Session["UserId"])).ToList();
            }
            return Json(Response);
        }

        [HttpPost]

        public ActionResult DeviceBulkUpload(HttpPostedFileBase fileUpload)
        {
            string UploadPath = Server.MapPath(ConfigurationManager.AppSettings["DevicePath"].ToString());
            string FilePath = string.Empty;
            DataSet dataSet = new DataSet();
            SQLDAL sQLDAL = new SQLDAL();
            string SQLConStr = ConfigurationManager.ConnectionStrings["MSSQL_ConnectionString"].ConnectionString;
            if (fileUpload != null)
            {

                if (!Directory.Exists(UploadPath))
                {
                    Directory.CreateDirectory(UploadPath);
                }

                FilePath = UploadPath + Path.GetFileName(fileUpload.FileName);
                string extension = Path.GetExtension(fileUpload.FileName);
                fileUpload.SaveAs(FilePath);
                string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + FilePath + "';Extended Properties='Excel 8.0;HDR=YES'";
                string conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                DataTable mdttable = new DataTable();

                switch (extension)
                {
                    case ".xls":
                        conString = string.Format(conString, FilePath);
                        mdttable = ForXLSExcelFile(conString);
                        break;

                    case ".xlsx":

                        conString = string.Format(constr, FilePath);
                        mdttable = ForXLSXExcelFile(conString);
                        break;

                    case ".csv":

                        mdttable = ForCSVFile(FilePath);
                        break;
                }

                SqlParameter[] sqlParameterCSV = new SqlParameter[3];
                sqlParameterCSV[0] = new SqlParameter("@tbl_UploadDevice", mdttable);
                sqlParameterCSV[1] = new SqlParameter("@mint_clientId", Convert.ToInt32(Request.Form["ClientId"]));
                sqlParameterCSV[2] = new SqlParameter("@EnteredBy", Convert.ToInt32(Session["UserId"]));
                
                dataSet = sQLDAL.ExecuteDataset(SQLConStr, "proc_InsertDeviceDetails_set", sqlParameterCSV);

                if (dataSet.Tables.Count > 1)
                {
                    if (dataSet.Tables[1].Rows.Count > 0)
                    {
                        ExportToExcel(dataSet.Tables[1], extension);
                        //string path = "<script>window.location.href='ExportToExcel(" + dataSet.Tables[1] + "," + extension + ")';</script>";
                        //ViewBag.DownloadRejectFile = "";
                    }
                }
            }
            return Json("");
        }

        DataTable ForXLSXExcelFile(string conString)
        {
            DataTable dt;

            using (OleDbConnection connExcel = new OleDbConnection(conString))
            {
                using (OleDbCommand cmdExcel = new OleDbCommand())
                {
                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                    {
                        cmdExcel.Connection = connExcel;

                        //Get the name of First Sheet.
                        connExcel.Open();
                        DataTable dtExcelSchema;
                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                        connExcel.Close();

                        //Read Data from First Sheet.
                        connExcel.Open();
                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                        odaExcel.SelectCommand = cmdExcel;
                        DataSet ds = new DataSet();
                        odaExcel.Fill(ds);
                        dt = ds.Tables[0];
                        connExcel.Close();
                        int cntrow = dt.Rows.Count;
                        int cntcolumn = dt.Columns.Count;
                        return dt;
                    }
                }

            }
        }

        DataTable ForXLSExcelFile(string conString)
        {
            DataTable dt;

            using (OleDbConnection connExcel = new OleDbConnection(conString))
            {
                using (OleDbCommand cmdExcel = new OleDbCommand())
                {
                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                    {
                        cmdExcel.Connection = connExcel;

                        //Get the name of First Sheet.
                        connExcel.Open();
                        DataTable dtExcelSchema;
                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                        connExcel.Close();

                        //Read Data from First Sheet.
                        connExcel.Open();
                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                        odaExcel.SelectCommand = cmdExcel;
                        DataSet ds = new DataSet();
                        odaExcel.Fill(ds);
                        dt = ds.Tables[0];
                        connExcel.Close();
                        int cntrow = dt.Rows.Count;
                        int cntcol = dt.Columns.Count;
                        return dt;
                    }
                }

            }
        }


        DataTable ForCSVFile(string filePath)
        {

            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }

                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    if (rows.Length > 1)
                    {
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i].Trim();
                        }
                        dt.Rows.Add(dr);
                    }
                }

            }
            int cntrow = dt.Rows.Count;
            int cntcolumn = dt.Columns.Count;

            return dt;
        }
        void ExportToExcel(DataTable ExceLTable, string Extension)
        {
            ExceLTable.TableName = "Rejected Roster";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ExceLTable);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= RosterReport(" + DateTime.Now.Date + ")" + Extension + "");

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }

        [HttpPost]
        public ActionResult WriteExcel()
        {

            string filetodownload = string.Empty;
            string response = "NoData";
            try
            {

                string startPath = Server.MapPath("~/Downloads/DeviceDetails/");
                if (Directory.Exists(startPath))
                {
                    foreach (string _file in Directory.GetFiles(startPath))
                    {
                        System.IO.File.Delete(_file);
                    }
                }




                filetodownload = "Device Report " + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".xlsx";
                DataTable dt = new DataTable();
                SqlConnection SqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["MSSQL_ConnectionString"].ConnectionString);
                SqlCommand Sqlcomrep = new SqlCommand();
                Sqlcomrep.Connection = SqlCon;
                Sqlcomrep.CommandType = CommandType.StoredProcedure;
                Sqlcomrep.CommandTimeout = 500;
                Sqlcomrep.CommandText = "DeviceMList_get";


                SqlDataAdapter sqlda = new SqlDataAdapter(Sqlcomrep);
                SqlCon.Open();
                sqlda.Fill(dt);
                SqlCon.Close();

                using (XLWorkbook wb = new XLWorkbook())
                {

                    wb.Worksheets.Add(dt, "Clients");
                    wb.SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/Downloads/DeviceDetails/" + filetodownload));
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

        public ActionResult DownloadExcel(string file)
        {
            //get the temp folder and file path in server

            string fullPath = Path.Combine(Server.MapPath("~/Downloads/DeviceDetails/"), file);

            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/vnd.ms-excel", file);
        }



        [HttpPost]
        public ActionResult ManageDevicePdf()
        {
            string FileName = Path.Combine(Server.MapPath("~/Downloads/DeviceDetails/"));
            string filetodownload = "Device Report " + DateTime.Now.Date.ToString("dd-MM-yyyy") + ".pdf";


            string pathString = System.IO.Path.Combine(FileName, filetodownload);
            var file = System.IO.File.Create(pathString);
            file.Close();
            FileStream stream = new FileStream(pathString, FileMode.OpenOrCreate);
            Document doc = new Document(PageSize.A4.Rotate(), 40f, 40f, 20f, 20f);
            PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);

            var F = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.WHITE);
            var H = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
            var P = new BaseColor(102, 51, 153);
            var I = FontFactory.GetFont(FontFactory.HELVETICA, 9, P);

            DataTable dt = new DataTable();
            SqlConnection SqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["MSSQL_ConnectionString"].ConnectionString);
            SqlCommand Sqlcomrep = new SqlCommand();
            Sqlcomrep.Connection = SqlCon;
            Sqlcomrep.CommandType = CommandType.StoredProcedure;
            Sqlcomrep.CommandTimeout = 500;
            Sqlcomrep.CommandText = "DeviceMList_get";

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
            cell0 = new PdfPCell(new Phrase("MANAGE DEVICES", F));
            cell0.HorizontalAlignment = Element.ALIGN_CENTER;
            cell0.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell0.PaddingTop = 5;
            cell0.PaddingBottom = 7;
            // cell0.Rowspan = 3;
            cell0.BackgroundColor = new BaseColor(0, 119, 142);
            table0.AddCell(cell0);

            ////////////////////////////////////////////////////////////
            

            PdfPTable table = new PdfPTable(19);
            table.WidthPercentage = 100;

            PdfPCell cell;
            cell = new PdfPCell(new Phrase("Sl.No", H));
            cell.Colspan = 1;
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("DEVICE SERIAL NO", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 2;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("DEVICE IEMI NO", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 3;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("CLIENT NAME", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 3;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

           

            cell = new PdfPCell(new Phrase("CLUSTER NAME", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 3;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);


            cell = new PdfPCell(new Phrase("SITE NAME", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 2;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);



            //cell = new PdfPCell(new Phrase("POST", H));
            //cell.PaddingBottom = 5;
            //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //cell.Colspan = 2;
            //cell.BackgroundColor = new BaseColor(0, 119, 142);
            //table.AddCell(cell);

            cell = new PdfPCell(new Phrase("DEVICE HOME LOCATION ", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 3;
            cell.BackgroundColor = new BaseColor(0, 119, 142);
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("DEVICE STATUS", H));
            cell.PaddingBottom = 5;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 2;
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


                 
                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("DeviceSerial").ToString(), I));
                cell.Colspan = 2;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("DeviceIMEINo").ToString(), I));
                cell.Colspan = 3;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("ClientName").ToString(), I));
                cell.Colspan = 3;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("ClusterName").ToString(), I));
                cell.Colspan = 3;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("SiteName").ToString(), I));
                cell.Colspan = 2;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("DeviceHomeLocation").ToString(), I));
                cell.Colspan = 3;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                



                //cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("Post").ToString(), I));
                //cell.Colspan = 2;
                //cell.PaddingBottom = 5;
                //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //table.AddCell(cell);

                cell = new PdfPCell(new Phrase(dt.Rows[i].Field<string>("DeviceState").ToString(), I));
                cell.Colspan = 2;
                cell.PaddingBottom = 5;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
            }
          



            // table0.SpacingAfter = 12.5f;
            table.SpacingBefore = 10f;
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

            string fullPath = Path.Combine(Server.MapPath("~/Downloads/DeviceDetails/"), file);

            //return the file for download, this is an Excel 
            //so I set the file content type to "application/vnd.ms-excel"
            return File(fullPath, "application/pdf", file);
        }
    }
}