using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FoodOrder.Shared;
using System.Data;
using MySql.Data.MySqlClient;

namespace FoodOrder.Models
{
    public class transactionWallet
    {
        public int TRANSACTIONID { get; set; }
        public DateTime? TRANSACTIONDATE { get; set; }
        public int TRANSACTIONAMOUNT { get; set; }
        public int CUSTID { get; set; }
    }
    public class transactionManager : BaseManager
    {
        public static List<transactionWallet> Gettransaction(string whereclause, MySqlConnection conn = null)
        {
            transactionWallet objtransaction = null;
            List<transactionWallet> lsttransaction = new List<transactionWallet>();
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                //MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                tryOpenConnection(connection);
                string sql = "select * from transaction ";
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
                                objtransaction = ReaderDatatransaction(reader);
                                lsttransaction.Add(objtransaction);
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
            return lsttransaction;
        }
        private static transactionWallet ReaderDatatransaction(MySqlDataReader reader)
        {

            transactionWallet objtransaction = new transactionWallet();
            objtransaction.TRANSACTIONID = Utility.IsValidInt(reader["TRANSACTIONID"]);
            objtransaction.TRANSACTIONDATE = Utility.IsValidDateTime(reader["TRANSACTIONDATE"]);
            objtransaction.CUSTID = Utility.IsValidInt(reader["CUSTID"]);
            objtransaction.TRANSACTIONAMOUNT = Utility.IsValidInt(reader["TRANSACTIONAMOUNT"]);
            return objtransaction;
        }
        public static string Savetransaction(transactionWallet objtransaction, MySqlConnection conn = null, MySqlTransaction trans = null)
        {
            string returnMessage = "";
            string stransactionID = "";
            stransactionID = objtransaction.TRANSACTIONID.ToString();
            var templsttransaction = Gettransaction("TRANSACTIONID='" + stransactionID + "'", conn);
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
                    if (templsttransaction.Count <= 0)
                    {
                        isEdit = false;
                        sql = @"INSERT INTO transaction(TRANSACTIONDATE,CUSTID,TRANSACTIONAMOUNT) VALUES (@TRANSACTIONDATE, @CUSTID,@TRANSACTIONAMOUNT)";
                    }
                    else
                    {
                        sql = @"Update transaction set TRANSACTIONID=@TRANSACTIONID,TRANSACTIONDATE=@TRANSACTIONDATE, CUSTID=@CUSTID, TRANSACTIONAMOUNT=@TRANSACTIONAMOUNT where TRANSACTIONID=@TRANSACTIONID";

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
                        command.Parameters.AddWithValue("@TRANSACTIONID", objtransaction.TRANSACTIONID);
                    }
                    command.Parameters.AddWithValue("@TRANSACTIONDATE", objtransaction.TRANSACTIONDATE);
                    command.Parameters.AddWithValue("@CUSTID", objtransaction.CUSTID);
                    command.Parameters.AddWithValue("@TRANSACTIONAMOUNT", objtransaction.TRANSACTIONAMOUNT);

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

        public static string Deletetransaction(string TRANSACTIONID, MySqlConnection conn = null)
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
                    sql = @"DELETE from transaction Where TRANSACTIONID = @TRANSACTIONID";
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@TRANSACTIONID", TRANSACTIONID);
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