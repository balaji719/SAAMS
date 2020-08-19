using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SAAMS.FilterModel
{
    public class SQLDAL
    {
        //string connection = @"Data Source=DESKTOP-037UEFO\SQLEXPRESS;Initial Catalog = Menusample; Integrated Security = True";

        public SqlConnection sqlcon;
        public SqlCommand sqlcom;
        public SqlDataAdapter sqlda;
        DataSet mds_result = new DataSet();

        public DataSet ExecuteDataset(string Connectionstr, string spname, SqlParameter[] sqlparam)
        {
            sqlcon = new SqlConnection(Connectionstr);

            sqlcon.Open();
            sqlcom = new SqlCommand(spname, sqlcon);
            sqlcom.CommandType = CommandType.StoredProcedure;

            foreach (SqlParameter sqlpr in sqlparam)
            {
                sqlcom.Parameters.Add(sqlpr);
            }

            sqlda = new SqlDataAdapter(sqlcom);
            sqlda.Fill(mds_result);
            sqlcon.Close();
            return mds_result;



        }

    }
}