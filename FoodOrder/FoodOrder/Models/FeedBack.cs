using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MySql.Data.MySqlClient;
using FoodOrder.Shared;

namespace FoodOrder.Models
{
    public class FeedBack
    {
        public int CUSTID { get; set; }
        public DateTime? FBDATE { get; set; }
        public int RATING { get; set; }
        public string FBDESCRIPTION { get; set; }

    }
    public class FeedBackManager : BaseManager
    {
        public static List<FeedBack> GetFeedback(string WhereClause, MySqlConnection conn = null)
        {
            FeedBack objfeedback = new FeedBack();
            List<FeedBack> lstfeedback = new List<FeedBack>();

            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                // MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                tryOpenConnection(connection);
                string sql = "select * from feedback ";
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
                                objfeedback = readerDataFeedBack(reader);
                                lstfeedback.Add(objfeedback);
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

            return lstfeedback;

        }

        private static FeedBack readerDataFeedBack(MySqlDataReader reader)
        {
            FeedBack objfeedBack = new FeedBack();
            objfeedBack.CUSTID = Utility.IsValidInt(reader["CUSTID"]);
            objfeedBack.FBDATE = Utility.IsValidDateTime(reader["FBDATE"]);
            objfeedBack.FBDESCRIPTION = Utility.IsValidString(reader["FBDESCRIPTION"]);
            objfeedBack.RATING = Utility.IsValidInt(reader["RATING"]);

            return objfeedBack;
        }
        public static string SaveFeedback(FeedBack objfeedback, MySqlConnection conn = null)
        {
            string returnMessage = "";
            string sCUSTID = "";

            sCUSTID = objfeedback.CUSTID.ToString();
            //var templstfeedback = GetFeedback("CUSTID='" + sCUSTID + "'", conn);
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                //MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                tryOpenConnection(connection);
                using (MySqlCommand command = new MySqlCommand())
                {
                    string sql;
                    bool isEdit = true;
                    //if (templstfeedback.Count <= 0)
                    //{
                    //    isEdit = false;
                        sql = @"Insert into Feedback(CUSTID,FBDATE,RATING,FBDESCRIPTION)  VALUES(@CUSTID,@FBDATE, @RATING, @FBDESCRIPTION )";
                    //}
                    //else
                    //{

                    //    sql = @"UPDATE FEEDBACK SET CUSTID=@CUSTID, FBDATE=@FBDATE, FBDESCRIPTION=@FBDESCRIPTION, RATING=@RATING";
                    //}
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    
                    command.Parameters.AddWithValue("@CUSTID", objfeedback.CUSTID);
                    command.Parameters.AddWithValue("@FBDATE", objfeedback.FBDATE);
                    command.Parameters.AddWithValue("@FBDESCRIPTION", objfeedback.FBDESCRIPTION);
                    command.Parameters.AddWithValue("@RATING", objfeedback.RATING);


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
        public static string Deletefeedback(string CUSTID, MySqlConnection conn = null)
        {
            string returnMessage = "";
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                //MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                tryOpenConnection(connection);
                using (MySqlCommand command = new MySqlCommand())
                {
                    string sql;
                    sql = @"Delete from feedback where CUSTID=@CUSTID";
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

    }
}