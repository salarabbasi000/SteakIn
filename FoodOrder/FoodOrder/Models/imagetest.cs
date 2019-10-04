using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using FoodOrder.Shared;
using MySql.Data.MySqlClient;

namespace FoodOrder.Models
{
    public class imagetest
    {
        public string itemname { get; set; }
        public string itemimage { get; set; }
        public HttpPostedFileBase IMAGEFILE { get; set; }

    }
    public class imagetestmanager : BaseManager
    {
        public static List<imagetest> Getimagetest(string WhereClause, MySqlConnection conn = null)
        {
            imagetest objimagetest = new imagetest();
            List<imagetest> lstimagetest = new List<imagetest>();

            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                // MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                tryOpenConnection(connection);
                string sql = "select * from imagetest ";
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
                                objimagetest = readerDataimagetest(reader);
                                lstimagetest.Add(objimagetest);
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

            return lstimagetest;

        }
        private static imagetest readerDataimagetest(MySqlDataReader reader)
        {
            imagetest objimage = new imagetest();
            objimage.itemname = Utility.IsValidString(reader["itemname"]);
            objimage.itemimage = Utility.IsValidString(reader["itemimage"]);



            return objimage;
        }
        public static string SaveImage(imagetest objimage, MySqlConnection conn = null, MySqlTransaction trans = null)
        {
            string returnMessage = "";
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                //MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                tryOpenConnection(connection);
            using (MySqlCommand command = new MySqlCommand())
            {
                string sql;
                sql = @"INSERT INTO imagetest(
                                    itemname,image) VALUES(@itemname,@itemimage)";


                if (trans != null)
                {
                    command.Transaction = trans;
                }
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                command.CommandText = sql;


                command.Parameters.AddWithValue("@itemname", objimage.itemname);

                command.Parameters.AddWithValue("@itemimage", objimage.itemimage);

                int affectedRows = command.ExecuteNonQuery();
                var lastInsertID = command.LastInsertedId;
                if (affectedRows > 0)
                {

                    returnMessage = "OK";


                }
                else
                {
                    returnMessage = Shared.Constants.MSG_ERR_DBSAVE.Text;
                }


                if (isConnArgNull == true)
                {
                    connection.Dispose();
                }
            }
           

            return returnMessage;
        }



    }
}