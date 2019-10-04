using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FoodOrder.Shared;
using System.Data;
using MySql.Data.MySqlClient;
namespace FoodOrder.Models
{
    public class wallet
    {
        public int WALLETID { get; set; }
        public int CUSTOMERID { get; set; }
        public int BALANCE { get; set; }
    }
    public class walletManager : BaseManager
    {
        public static List<wallet> Getwallet(string whereclause, MySqlConnection conn = null)
        {
            wallet objwallet = null;
            List<wallet> lstwallet = new List<wallet>();
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                //MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                tryOpenConnection(connection);
                string sql = "select * from wallet ";
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
                                objwallet = ReaderDatawallet(reader);
                                lstwallet.Add(objwallet);
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
            return lstwallet;
        }
        private static wallet ReaderDatawallet(MySqlDataReader reader)
        {

            wallet objwallet = new wallet();

            objwallet.WALLETID = Utility.IsValidInt(reader["WALLETID"]);
            objwallet.CUSTOMERID = Utility.IsValidInt(reader["CUSTOMERID"]);
            objwallet.BALANCE = Utility.IsValidInt(reader["BALANCE"]);
            return objwallet;
        }
        public static string Savewallet(wallet objwallet, MySqlConnection conn = null, MySqlTransaction trans = null)
        {
            string returnMessage = "";
            string sWALLETID = "";
            sWALLETID = objwallet.WALLETID.ToString();
            var templstwallet = Getwallet("WALLETID='" + sWALLETID + "'", conn);
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
                    if (templstwallet.Count <= 0)
                    {
                        isEdit = false;
                        sql = @"INSERT INTO wallet(CUSTOMERID,BALANCE ) VALUES (@CUSTOMERID,@BALANCE)";
                    }
                    else
                    {
                        sql = @"Update WALLET set WALLETID=@WALLETID,CUSTOMERID=@CUSTOMERID, BALANCE=@BALANCE where WALLETID=@WALLETID";

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
                        command.Parameters.AddWithValue("WALLETID", objwallet.WALLETID);
                    }
                    command.Parameters.AddWithValue("@CUSTOMERID", objwallet.CUSTOMERID);
                    command.Parameters.AddWithValue("@BALANCE", objwallet.BALANCE);
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

        public static string Deletewallet(string WALLETID, MySqlConnection conn = null)
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
                    sql = @"DELETE from wallet Where WALLETID = @WALLETID";
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@WALLETID", WALLETID);
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