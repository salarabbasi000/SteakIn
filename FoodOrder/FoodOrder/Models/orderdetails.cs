using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FoodOrder.Shared;
using System.Data;
using MySql.Data.MySqlClient;

namespace FoodOrder.Models
{
    public class orderdetails
    {
        public int ORDERID { get; set; }
        public int ITEMID { get; set; }
        public int QUANTITY { get; set; }

        //OTHER VAR
        public int AMOUNTSUBTOTAL { get; set; }
        public int PRICE { get; set; }
        public string ITEMNAME { get; set; }
        public int TOTAL { get; set; }


    }
    public class orderdetailsManager : BaseManager
    {
        public static List<orderdetails> Getorderdetails(string whereclause, MySqlConnection conn = null)
        {
            orderdetails objorderdetails = new orderdetails();
            List<orderdetails> lstorderdetails = new List<orderdetails>();
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                tryOpenConnection(connection);
                string sql = "select * from orderdetails ";
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
                                objorderdetails = ReaderDataorderdetails(reader);
                                lstorderdetails.Add(objorderdetails);
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
            return lstorderdetails;
        }
        private static orderdetails ReaderDataorderdetails(MySqlDataReader reader)
        {
            orderdetails objorderdetails = new orderdetails();
            objorderdetails.ORDERID = Utility.IsValidInt(reader["ORDERID"]);
            objorderdetails.ITEMID = Utility.IsValidInt(reader["ITEMID"]);
            objorderdetails.QUANTITY = Utility.IsValidInt(reader["QUANTITY"]);


            return objorderdetails;
        }

        public static string Saveorderdetails(orderdetails objorderdetails, MySqlConnection conn = null, MySqlTransaction trans = null)
        {
            string returnMessage = "";
            //string sORDERID = "";
            //sORDERID = objorderdetails.ORDERID.ToString();
            //var templstorderdetails = Getorderdetails("ORDERID='" + sORDERID + "'", conn);
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                //MySqlConnection connection = new MySqlConnection("server=32.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                tryOpenConnection(connection);
                using (MySqlCommand command = new MySqlCommand())
                {
                    string sql;
                  
                        sql = @"INSERT INTO ORDERDETAILS(ORDERID,ITEMID,QUANTITY) VALUES (@ORDERID,@ITEMID,@QUANTITY)";
                    
                   
                    if (trans != null)
                    {
                        command.Transaction = trans;
                    }
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                  
                    command.Parameters.AddWithValue("@ORDERID", objorderdetails.ORDERID);
                    command.Parameters.AddWithValue("@ITEMID", objorderdetails.ITEMID);
                    command.Parameters.AddWithValue("@QUANTITY", objorderdetails.QUANTITY);

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

        public static string Deleteorderdetails(string ORDERID, MySqlConnection conn = null)
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
                    sql = @"DELETE from orderdetails Where ORDERID = @ORDERID";
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@ORDERID", ORDERID);
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

        public static List<orderdetails> GetOrderDetailsItem(string ORDERID, MySqlConnection conn=null)
        {
            orderdetails objorderdetails = null;
            List<orderdetails> lstorderdetails = new List<orderdetails>();
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                tryOpenConnection(connection);
                string sql = "SELECT        D.ORDERID, F.ITEMNAME, F.PRICE, SUM(D.QUANTITY) AS QUANTITY, SUM(D.QUANTITY * F.PRICE) AS SUBTOTAL FROM orderdetails D INNER JOIN fooditem F ON D.ITEMID = F.ITEMID WHERE (D.ORDERID= "+(ORDERID)+")    GROUP BY D.ORDERID, F.ITEMNAME";
                //if (!string.IsNullOrEmpty(whereclause))
                //    sql += " where " + whereclause;
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
                                objorderdetails = ReaderDataOrderDetailsItem(reader);
                                lstorderdetails.Add(objorderdetails);
                               
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
            catch (Exception ex)
            {
            }

            return lstorderdetails;
        }
        private static orderdetails ReaderDataOrderDetailsItem(MySqlDataReader reader)
        {
            orderdetails objorderdetails = new orderdetails();
            objorderdetails.ITEMNAME = Utility.IsValidString(reader["ITEMNAME"]);
            objorderdetails.PRICE = Utility.IsValidInt(reader["PRICE"]);
            objorderdetails.QUANTITY = Utility.IsValidInt(reader["QUANTITY"]);
            objorderdetails.AMOUNTSUBTOTAL = Utility.IsValidInt(reader["SUBTOTAL"]);

            return objorderdetails;
        }
        public static orderdetails getTotalAmount(string ORDERID, MySqlConnection conn = null)
        {
            orderdetails objorderdetails = null;
            List<orderdetails> lstorderdetails = new List<orderdetails>();
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                tryOpenConnection(connection);
                string sql = "SELECT D.ORDERID, SUM(D.QUANTITY * F.PRICE) AS TOTAL FROM orderdetails D INNER JOIN fooditem F ON D.ITEMID = F.ITEMID WHERE (D.ORDERID= " + (ORDERID) + ") GROUP BY D.ORDERID";
                //if (!string.IsNullOrEmpty(whereclause))
                //    sql += " where " + whereclause;
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
                                objorderdetails = ReaderDataOrderTotalAmount(reader);
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
            catch (Exception ex)
            {
            }

            return objorderdetails;

        }
        private static orderdetails ReaderDataOrderTotalAmount(MySqlDataReader reader)
        {
            orderdetails objorderdetails = new orderdetails();
            objorderdetails.TOTAL = Utility.IsValidInt(reader["TOTAL"]);

            return objorderdetails;
        }

    }

}