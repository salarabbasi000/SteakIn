using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MySql.Data.MySqlClient;
using FoodOrder.Shared;


namespace FoodOrder.Models
{
    public class mastercard
    {
        public int CARDID { get; set; }
        public DateTime? EXPIRYDATE { get; set; }
        public int CVV { get; set; }
        public int CUSTID { get; set; }
        public int PIN { get; set; }

    }
    public class mastercardManager : BaseManager
    {
        public static List<mastercard> Getmastercard(string whereclause, MySqlConnection conn = null)
        {
            mastercard objmastercard = null;
            List<mastercard> lstmastercard = new List<mastercard>();
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                //MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                tryOpenConnection(connection);
                string sql = "select * from mastercard ";
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
                                objmastercard = ReaderDatamastercard(reader);
                                lstmastercard.Add(objmastercard);
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
            return lstmastercard;
        }
        private static mastercard ReaderDatamastercard(MySqlDataReader reader)
        {

            mastercard objmastercard = new mastercard();
            objmastercard.CARDID = Utility.IsValidInt(reader["CARDID"]);
            objmastercard.EXPIRYDATE = Utility.IsValidDateTime(reader["EXPIRYDATE"]);
            objmastercard.CVV = Utility.IsValidInt(reader["CVV"]);
            objmastercard.CUSTID = Utility.IsValidInt(reader["CUSTID"]);
            objmastercard.PIN = Utility.IsValidInt(reader["PIN"]);
            return objmastercard;
        }
        public static string Savemastercard(mastercard objmastercard, MySqlConnection conn = null, MySqlTransaction trans = null)
        {
            string returnMessage = "";
            string sCARDID = "";
            sCARDID = objmastercard.CARDID.ToString();
            var templstmastercard = Getmastercard("CARDID='" + sCARDID + "'", conn);
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
                    if (templstmastercard.Count <= 0)
                    {
                        isEdit = false;
                        sql = @"INSERT INTO mastercard(CARDID,EXPIRYDATE,CVV,CUSTID,PIN ) VALUES (@CARDID,@EXPIRYDATE,@CVV,@CUSTID,@PIN)";
                    }
                    else
                    {
                        sql = @"Update mastercard set CARDID=@CARDID,EXPIRYDATE=@EXPIRYDATE, CVV=@CVV,CUSTID=@CUSTID,PIN=@PIN where CARDID=@CARDID";

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
                        command.Parameters.AddWithValue("@CARDID", objmastercard.CARDID);
                    }
                    command.Parameters.AddWithValue("@EXPIRYDATE", objmastercard.EXPIRYDATE);
                    command.Parameters.AddWithValue("@CVV", objmastercard.CVV);
                    command.Parameters.AddWithValue("@CUSTID", objmastercard.CUSTID);
                    command.Parameters.AddWithValue("@PIN", objmastercard.PIN);


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

        public static string Deletemastercard(string CARDID, MySqlConnection conn = null)
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
                    sql = @"DELETE from mastercard Where CARDID = @CARDID";
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@CARDID", CARDID);
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