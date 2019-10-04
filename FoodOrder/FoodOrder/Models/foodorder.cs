using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data;
using FoodOrder.Models;
using FoodOrder.Shared;
namespace FoodOrder.Models
{   
  

    public class foodorder
    {
        public int ORDERID { get; set; }
        public DateTime? ORDERDATE { get; set; }
        public int CUSTID { get; set; }
        public int AMOUNT { get; set; }
        public string PAYMENTMETHOD { get; set; }
        public string ORDERSTATUS { get; set; }


        //OTHER VAR
        public List<orderdetails> lstorderdetails;
        public int TOTALAMOUNT { get; set; }
        public string CUSTNAME { get; set; }
    }
    public class foodorderManager : BaseManager
    {
        public static List<foodorder> Getorder(string whereclause, MySqlConnection conn = null)
        {
            foodorder objfoodorder = new foodorder();
            List<foodorder> lstfoodorder = new List<foodorder>();
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                tryOpenConnection(connection);
                string sql = "select * from foodorder ";
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
                                objfoodorder = ReaderDatafoodorder(reader);
                                lstfoodorder.Add(objfoodorder);
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
            return lstfoodorder;
        }
        private static foodorder ReaderDatafoodorder(MySqlDataReader reader)
        {
            foodorder objfoodorder = new foodorder();
            objfoodorder.ORDERID = Utility.IsValidInt(reader["ORDERID"]);
            objfoodorder.ORDERDATE = Utility.IsValidDateTime(reader["ORDERDATE"]);
            objfoodorder.CUSTID = Utility.IsValidInt(reader["CUSTID"]);
            objfoodorder.AMOUNT = Utility.IsValidInt(reader["AMOUNT"]);
            objfoodorder.PAYMENTMETHOD = Utility.IsValidString(reader["PAYMENTMETHOD"]);
            objfoodorder.ORDERSTATUS = Utility.IsValidString(reader["ORDERSTATUS"]);

            return objfoodorder;
        }

        public static string Savefoodorder(foodorder objfoodorder, MySqlConnection conn = null, MySqlTransaction trans = null)
        {
            string returnMessage = "";
            string sORDERID = "";
            sORDERID = objfoodorder.ORDERID.ToString();
            var templstfoodorder = Getorder("ORDERID='" + sORDERID + "'", conn);
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                //MySqlConnection connection = new MySqlConnection("server=32.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                tryOpenConnection(connection);
                using (MySqlCommand command = new MySqlCommand())
                {
                    string sql;
                    bool isEdit = true;
                    if (templstfoodorder.Count <= 0)
                    {
                        isEdit = false;
                        sql = @"INSERT INTO FOODORDER(ORDERDATE, CUSTID,AMOUNT,PAYMENTMETHOD,ORDERSTATUS) VALUES (@ORDERDATE, @CUSTID, @AMOUNT, @PAYMENTMETHOD,@ORDERSTATUS)";
                    }
                    else
                    {
                        sql = @"Update FOODORDER set ORDERID=@ORDERID,ORDERDATE=@ORDERDATE, CUSTID=@CUSTID, AMOUNT=@AMOUNT, PAYMENTMETHOD=@PAYMENTMETHOD,ORDERSTATUS=@ORDERSTATUS where ORDERID=@ORDERID";

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
                        command.Parameters.AddWithValue("@ORDERID", objfoodorder.ORDERID);
                    }
                    command.Parameters.AddWithValue("@ORDERDATE", objfoodorder.ORDERDATE);
                    command.Parameters.AddWithValue("@CUSTID", objfoodorder.CUSTID);
                    command.Parameters.AddWithValue("@AMOUNT", objfoodorder.AMOUNT);
                    command.Parameters.AddWithValue("@PAYMENTMETHOD", objfoodorder.PAYMENTMETHOD);
                    command.Parameters.AddWithValue("@ORDERSTATUS", objfoodorder.ORDERSTATUS);
                    int affectedRows = command.ExecuteNonQuery();
                    var lastinsertedid = command.LastInsertedId;
                    if (affectedRows > 0)
                    {
                        if (!isEdit)
                        {
                            returnMessage = lastinsertedid.ToString();
                        }
                        else { returnMessage = "OK"; }
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

        public static string Deletefoodorder(string ORDERID, MySqlConnection conn = null)
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
                    sql = @"DELETE from FOODORDER Where ORDERID = @ORDERID";
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
        public static List<foodorder> GetOrderCustomer(string CUSTID, MySqlConnection conn = null)
        {
            foodorder objfoodorder = null;
            List<foodorder> lstfoodorder = new List<foodorder>();
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                tryOpenConnection(connection);
                string sql = "SELECT        O.ORDERID, C.CUSTNAME, O.ORDERDATE, O.PAYMENTMETHOD FROM customer C INNER JOIN foodorder O ON C.CUSTID = O.CUSTID WHERE (O.CUSTID= " + (CUSTID) + ")   ";
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
                                objfoodorder = ReaderDataOrderCustomer(reader);
                               lstfoodorder.Add(objfoodorder);

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

            return lstfoodorder;
        }
        private static foodorder ReaderDataOrderCustomer(MySqlDataReader reader)
        {
            foodorder foodorder= new foodorder();
            foodorder.ORDERID = Utility.IsValidInt(reader["ORDERID"]);
            foodorder.ORDERDATE = Utility.IsValidDateTime(reader["ORDERDATE"]);
            foodorder.CUSTNAME = Utility.IsValidString(reader["CUSTNAME"]);
            foodorder.PAYMENTMETHOD = Utility.IsValidString(reader["PAYMENTMETHOD"]);
            foodorder.AMOUNT = getTotalAmount(foodorder.ORDERID.ToString());

            return foodorder;
        }

        public static int getTotalAmount(string OrderId, MySqlConnection conn = null)
        { int Amount;
            orderdetails obj = new orderdetails();
            obj = orderdetailsManager.getTotalAmount(OrderId, conn);
            Amount = obj.TOTAL;


            return Amount;
        }

    }



}