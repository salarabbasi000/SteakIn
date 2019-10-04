using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FoodOrder.Shared;
using System.Data;
using MySql.Data.MySqlClient;
namespace FoodOrder.Models
{
    public class admin
    {
        public int USERID { get; set; }
        public string ADMINUSERNAME { get; set; }
        public string PASSWORD { get; set; }
        public string ROLE { get; set; }
    }
    public class adminManager : BaseManager
    {
        public static List<admin> Getadmin(string whereclause, MySqlConnection conn = null)
        {
            admin objadmin = null;
            List<admin> lstadmin = new List<admin>();
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                //MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                tryOpenConnection(connection);
                string sql = "select * from admin ";
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
                                objadmin = ReaderDataadmin(reader);
                                lstadmin.Add(objadmin);
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
            return lstadmin;
        }
        private static admin ReaderDataadmin(MySqlDataReader reader)
        {

            admin objadmin = new admin();
            objadmin.USERID = Utility.IsValidInt(reader["USERID"]);
            objadmin.PASSWORD = Utility.IsValidString(reader["PASSWORD"]);
            objadmin.ROLE = Utility.IsValidString(reader["ROLE"]);
            objadmin.ADMINUSERNAME = Utility.IsValidString(reader["ADMINUSERNAME"]);


            return objadmin;
        }
        public static string Saveadmin(admin objadmin, MySqlConnection conn = null, MySqlTransaction trans = null)
        {
            string returnMessage = "";
            string sUSERID = "";
            sUSERID = objadmin.USERID.ToString();
            var templstadmin = Getadmin("USERID='" + sUSERID + "'", conn);
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
                    if (templstadmin.Count <= 0)
                    {
                        isEdit = false;
                        sql = @"INSERT INTO admin(USERID,PASSWORD,ROLE,ADMINUSERNAME ) VALUES (@USERID,@PASSWORD,@ROLE,@ADMINUSERNAME)";
                    }
                    else
                    {
                        sql = @"Update admin set USERID=@USERID,PASSWORD=@PASSWORD, ROLE=@ROLE , ADMINUSERNAME=@ADMINUSERNAME where USERID=@USERID";

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
                        command.Parameters.AddWithValue("@USERID", objadmin.USERID);
                    }
                    command.Parameters.AddWithValue("PASSWORD", objadmin.PASSWORD);
                    command.Parameters.AddWithValue("ROLE", objadmin.ROLE);
                    command.Parameters.AddWithValue("ADMINUSERNAME", objadmin.ADMINUSERNAME);


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

        public static string Deleteadmin(string USERID, MySqlConnection conn = null)
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
                    sql = @"DELETE from admin Where USERID = @USERID";
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@USERID", USERID);
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