using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using FoodOrder.Shared;
using MySql.Data.MySqlClient;

namespace FoodOrder.Models
{

    public class ItemCategory
    {
        public int CATEGORYID { get; set; }
        public string CATEGORYNAME { get; set; }

    }
    public class ItemCategoryManager : BaseManager
    {
        public static List<ItemCategory> GetItemCategories(string whereclause, MySqlConnection conn = null)
        {
            ItemCategory objitemcategory = null;
            List<ItemCategory> lstitemCategories = new List<ItemCategory>();
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                //MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                tryOpenConnection(connection);
                string sql = "select * from itemcategory ";
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
                                objitemcategory = ReaderDataitemcategory(reader);
                                lstitemCategories.Add(objitemcategory);
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
            return lstitemCategories;
        }
        private static ItemCategory ReaderDataitemcategory(MySqlDataReader reader)
        {
            ItemCategory objitemCategory = new ItemCategory();
            objitemCategory.CATEGORYID = Utility.IsValidInt(reader["CATEGORYID"]);
            objitemCategory.CATEGORYNAME = Utility.IsValidString(reader["CATEGORYNAME"]);
            return objitemCategory;
        }
        public static string SaveItem(ItemCategory objitemcategory, MySqlConnection conn = null, MySqlTransaction trans = null)
        {
            string returnMessage = "";
            string sCATEGORYID = "";
            sCATEGORYID = objitemcategory.CATEGORYID.ToString();
            var templstitemcategory = GetItemCategories("CATEGORYID='" + sCATEGORYID + "'", conn);
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                //MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                tryOpenConnection(connection);
                using (MySqlCommand command = new MySqlCommand())
                {
                    string sql;
                    bool isEdit = true;
                    if (templstitemcategory.Count <= 0)
                    {
                        isEdit = false;
                        sql = @"INSERT INTO ITEMCATEGORY(CATEGORYID,CATEGORYNAME) VALUES (@CATEGORYID,@CATEGORYNAME)";
                    }
                    else
                    {
                        sql = @"Update itemcategory set CATEGORYID=@CATEGORYID,CATEGORYNAME=@CATEGORYNAME where CATEGORYID=@CATEGORYID";

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
                        command.Parameters.AddWithValue("@CATEGORYID", objitemcategory.CATEGORYID);
                    }
                    command.Parameters.AddWithValue("@CATEGORYNAME", objitemcategory.CATEGORYNAME);
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

        public static string Deleteitemcategory(string CATEGORYID, MySqlConnection conn = null)
        {
            string returnMessage = "";
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                //MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                tryOpenConnection(connection);
                using (MySqlCommand command = new MySqlCommand())
                {
                    string sql;
                    sql = @"DELETE from itemcategory Where CATEGORYID = @CATEGORYID";
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@CATEGORYID", CATEGORYID);
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