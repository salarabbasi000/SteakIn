using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MySql.Data.MySqlClient;
using FoodOrder.Shared;
using System.Data;

namespace FoodOrder.Models
{
    public class Customer
    {
        public int CUSTID { get; set; }
        public string CUSTNAME { get; set; }
        public string PASSWORD { get; set; }
        public string CUSTUSERNAME { get; set; }
        public string EMAIL { get; set; }

        //other
        private orderdetails objorder { get; set; }
        public int walletid { get; set; }
        public int balance { get; set; }

    }
    public class CustomerManager : BaseManager
    {
        public static List<Customer> GetCustomer(string WhereClause, MySqlConnection conn = null)
        {
            Customer objCustomer = null;
            List<Customer> lstCustomer = new List<Customer>();

            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
              //  MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                tryOpenConnection(connection);
                string sql = "select * from customer ";
                if (!string.IsNullOrEmpty(WhereClause))
                    sql += "where " + WhereClause;
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
                                objCustomer = readerDataCustomer(reader);
                                lstCustomer.Add(objCustomer);

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

            return lstCustomer;


        }

        private static Customer readerDataCustomer(MySqlDataReader reader)
        {
            Customer objcustomer = new Customer();
            objcustomer.CUSTID = Utility.IsValidInt(reader["CUSTID"]);
            objcustomer.CUSTNAME = Utility.IsValidString(reader["CUSTNAME"]);
            objcustomer.PASSWORD = Utility.IsValidString(reader["PASSWORD"]);
            objcustomer.EMAIL = Utility.IsValidString(reader["EMAIL"]);
            objcustomer.CUSTUSERNAME= Utility.IsValidString(reader["CUSTUSERNAME"]);

            return objcustomer;
        }
        public static string SaveCustomer(Customer objcustomer, MySqlConnection conn = null, MySqlTransaction trans = null)
        {
            string returnMessage = "";
            string sCUSTID = "";
            sCUSTID = objcustomer.CUSTID.ToString();
            var templstcustomer = GetCustomer("CUSTID='" + sCUSTID + "'", conn);
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                //MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                tryOpenConnection(connection);
                using (MySqlCommand command = new MySqlCommand())
                {
                    string sql;
                    bool isEdit = true;
                    if (templstcustomer.Count <= 0)
                    {
                        isEdit = false;
                        sql = @"Insert into Customer( CUSTNAME, PASSWORD, CUSTUSERNAME, EMAIL)  VALUES( @CUSTNAME, @PASSWORD,@CUSTUSERNAME, @EMAIL)";
                    }
                    else
                    {

                        sql = @"UPDATE CUSTOMER SET
                           CUSTID=@CUSTID,
                            CUSTNAME=@CUSTNAME,
                            PASSWORD=@PASSWORD,
                            CUSTUSERNAME=@CUSTUSERNAME,EMAIL=@EMAIL";
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
                        command.Parameters.AddWithValue("@CUSTID", objcustomer.CUSTID);
                    }
                    command.Parameters.AddWithValue("@CUSTNAME", objcustomer.CUSTNAME);
                    command.Parameters.AddWithValue("@PASSWORD", objcustomer.PASSWORD);
                    command.Parameters.AddWithValue("@CUSTUSERNAME", objcustomer.CUSTUSERNAME);
                    command.Parameters.AddWithValue("@EMAIL", objcustomer.EMAIL);
               
                    
                    int affectedRows = command.ExecuteNonQuery();
                    var lastInsertID = command.LastInsertedId;
                    if (affectedRows > 0)
                    {
                        if (!isEdit)
                        {
                            returnMessage = lastInsertID.ToString();
                        }
                        else
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
        public static string DeleteCustomer(string CUSTID, MySqlConnection conn = null)
        {
            string returnMessage = "";
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                //MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                tryOpenConnection(connection);
                using (MySqlCommand command = new MySqlCommand())
                {
                    string sql;
                    sql = @"Delete from customer where CUSTID=@CUSTID";
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@CUSTID", CUSTID);
                    int affectedRows = command.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        returnMessage = "OK";
                    }
                    else
                    {
                        returnMessage = "Unable to save, Please contact ISD";
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

        public static List<Customer> GetCustomerWallet(string WhereClause, MySqlConnection conn = null)
        {
            Customer objCustomer = null;
            List<Customer> lstCustomer = new List<Customer>();

            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                //MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                tryOpenConnection(connection);
                string sql = " SELECT C.CUSTID, C.CUSTNAME, C.EMAIL, C.CUSTUSERNAME, W.WALLETID, W.BALANCE FROM customer C INNER JOIN wallet W ON C.CUSTID = W.CUSTOMERID";
                if (!string.IsNullOrEmpty(WhereClause))
                    sql += " where " + WhereClause;
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
                                objCustomer = readerDataCustomerWallet(reader);
                                lstCustomer.Add(objCustomer);

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

            return lstCustomer;


        }

        private static Customer readerDataCustomerWallet(MySqlDataReader reader)
        {
            Customer objcustomer = new Customer();
            objcustomer.CUSTID = Utility.IsValidInt(reader["CUSTID"]);
            objcustomer.CUSTNAME = Utility.IsValidString(reader["CUSTNAME"]);
            
            objcustomer.EMAIL = Utility.IsValidString(reader["EMAIL"]);
            objcustomer.CUSTUSERNAME = Utility.IsValidString(reader["CUSTUSERNAME"]);
            objcustomer.walletid = Utility.IsValidInt(reader["WALLETID"]);
            objcustomer.balance = Utility.IsValidInt(reader["BALANCE"]);

            return objcustomer;
        }



    }
}