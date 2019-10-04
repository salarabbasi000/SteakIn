using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FoodOrder.Shared;
using System.Data;
using MySql.Data.MySqlClient;


namespace FoodOrder.Models
{
    public class payment
    {
        public int PAYMENTID { get; set; }
        public int ORDERID { get; set; }
        public DateTime? PAYMENTDATE { get; set; }

    }
    public class paymentManager : BaseManager
    {
        public static List<payment> GetPayment(string whereclause, MySqlConnection conn = null)
        {
            payment objpayment = null;
            List<payment> lstpayment = new List<payment>();
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                //MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                tryOpenConnection(connection);
                string sql = "select * from payment ";
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
                                objpayment = ReaderDatapayment(reader);
                                lstpayment.Add(objpayment);
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
            return lstpayment;
        }
        private static payment ReaderDatapayment(MySqlDataReader reader)
        {

            payment objpayment = new payment();
            objpayment.PAYMENTID = Utility.IsValidInt(reader["PAYMNETID"]);
            objpayment.PAYMENTDATE = Utility.IsValidDateTime(reader["PAYMENTDATE"]);
            objpayment.ORDERID = Utility.IsValidInt(reader["ORDERID"]);
            return objpayment;
        }
        public static string Savepayment(payment objpayment, MySqlConnection conn = null, MySqlTransaction trans = null)
        {
            string returnMessage = "";
            string sPAYMENTID = "";
            sPAYMENTID = objpayment.PAYMENTID.ToString();
            var templstpayment = GetPayment("PAYMENTID='" + sPAYMENTID + "'", conn);
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                // MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                tryOpenConnection(connection);
                using (MySqlCommand command = new MySqlCommand())
                {
                    string sql;
                    bool isEdit = true;
                    if (templstpayment.Count <= 0)
                    {
                        isEdit = false;
                        sql = @"INSERT INTO PAYMENT(ORDERID,PAYMENTDATE ) VALUES (@ORDERID,@PAYMENTDATE)";
                    }
                    else
                    {
                        sql = @"Update PAYMENT set PAYMENTID=@PAYMENTID,ORDERID=@ORDERID, PAYMENTDATE=@PAYMENTDATE where PAYMENTID=@PAYMENTID";

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
                        command.Parameters.AddWithValue("@PAYMENTID", objpayment.PAYMENTID);
                    }
                    command.Parameters.AddWithValue("@ORDERID", objpayment.ORDERID);
                    command.Parameters.AddWithValue("@PAYMENTDATE", objpayment.PAYMENTDATE);
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

        public static string Deletepayment(string PAYMENTID, MySqlConnection conn = null)
        {
            string returnMessage = "";
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                // MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                tryOpenConnection(connection);
                using (MySqlCommand command = new MySqlCommand())
                {
                    string sql;
                    sql = @"DELETE from PAYMENT Where PAYMENTID = @PAYMENTID";
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@PAYMENTID", PAYMENTID);
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
