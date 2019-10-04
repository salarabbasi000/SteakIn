using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FoodOrder.Shared;
using System.Data;
using MySql.Data.MySqlClient;

namespace FoodOrder.Models
{
    public class paymentmethod
    {
        public int PAYMENTMETHODID { get; set; }
        public string METHODTYPE { get; set; }
    }

    public class paymentmethodManager : BaseManager
    {
        public static List<paymentmethod> Getpaymentmethod(string whereclause, MySqlConnection conn = null)
        {
            paymentmethod objpaymentmethod = null;
            List<paymentmethod> lstpaymentmethod = new List<paymentmethod>();
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                //MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                tryOpenConnection(connection);
                string sql = "select * from paymentmethod ";
                if (!string.IsNullOrEmpty(whereclause))
                    sql += " where " + whereclause;
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sql;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                objpaymentmethod = ReaderDatapaymentmethod(reader);
                                lstpaymentmethod.Add(objpaymentmethod);
                            }
                        }
                        else
                        {
                        }
                    }
                }

                if (isConnArgNull == true)
                {
                    connection.Dispose();
                }

            }
            catch
            {

            }
            return lstpaymentmethod;
        }
        private static paymentmethod ReaderDatapaymentmethod(MySqlDataReader reader)
        {

            paymentmethod objpaymentmethod = new paymentmethod();
            objpaymentmethod.PAYMENTMETHODID = Utility.IsValidInt(reader["PAYMENTMETHODID"]);
            objpaymentmethod.METHODTYPE = Utility.IsValidString(reader["METHODTYPE"]);

            return objpaymentmethod;
        }
        public static string Savepaymentmethod(paymentmethod objpaymentmethod, MySqlConnection conn = null, MySqlTransaction trans = null)
        {
            string returnMessage = "";
            string sPAYMENTMETHODID = "";
            sPAYMENTMETHODID = objpaymentmethod.PAYMENTMETHODID.ToString();
            var templstpaymentmethod = Getpaymentmethod("PAYMENTMETHODID='" + sPAYMENTMETHODID + "'", conn);
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                //MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                tryOpenConnection(connection);
                using (MySqlCommand command = new MySqlCommand())
                {
                    string sql;
                    bool isEdit = true;
                    if (templstpaymentmethod.Count <= 0)
                    {
                        isEdit = false;
                        sql = @"INSERT INTO PAYMENTMETHOD(PAYMENTMETHODID,METHODTYPE) VALUES (@PAYMENTMETHODID,@METHODTYPE)";
                    }
                    else
                    {
                        sql = @"Update PAYMENTMETHOD set PAYMENTMETHODID=@PAYMENTMETHODID,METHODTYPE=@METHODTYPE where PAYMENTMETHODID=@PAYMENTMETHODID";

                    }
                    if (trans != null)
                    {
                        command.Transaction = trans;
                    }
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    if (isEdit)
                    {
                        command.Parameters.AddWithValue("@PAYMENTMETHODID", objpaymentmethod.PAYMENTMETHODID);
                    }
                    command.Parameters.AddWithValue("METHODTYPE", objpaymentmethod.METHODTYPE);
                    int affectedRows = command.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        returnMessage = "OK";
                    }
                    else
                    {
                        returnMessage = Constants.MSG_ERR_DBSAVE.Text;
                    }
                }

                if (isConnArgNull == true)
                {
                    connection.Dispose();
                }
            }
            catch (Exception ex)
            {
            }

            return returnMessage;
        }

        public static string Deletepaymentmethod(string PAYMENTMETHODID, MySqlConnection conn = null)
        {
            string returnMessage = "";
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                //MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                tryOpenConnection(connection);
                using (MySqlCommand command = new MySqlCommand())
                {
                    string sql;
                    sql = @"DELETE from paymentmethod Where PAYMENTMETHODID = @PAYMENTMETHODID";
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@PAYMENTMETHODID", PAYMENTMETHODID);
                    int affectedRows = command.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        returnMessage = "OK";
                    }
                    else
                    {
                        returnMessage = Shared.Constants.MSG_ERR_DBSAVE.Text;
                    }
                }

                if (isConnArgNull == true)
                {
                    connection.Dispose();
                }
            }
            catch (Exception ex)
            {
            }

            return returnMessage;

        }

    }
}