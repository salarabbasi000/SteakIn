using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MySql.Data.MySqlClient;
using FoodOrder.Shared;
using System.Data;
using System.ComponentModel;

namespace FoodOrder.Models
{
    public class fooditem
    {
        public int ITEMID { get; set; }
        public string ITEMNAME { get; set; }
        public int PRICE { get; set; }
        public string ITEMDESCRIPTION { get; set; }
        public string ITEMCATEGORY { get; set; }
        public string ITEMSTATUS { get; set; }


        //IMAGE
        [DisplayName("Upload File")]
        public string IMAGEP { get; set; }
        public HttpPostedFileBase IMAGEFILE { get; set;}




    }
    public class fooditemManager : BaseManager
    {
        public static List<fooditem> Getfooditem(string WhereClause, MySqlConnection conn = null)
        {
            fooditem objfooditem = null;
            List<fooditem> lstfooditem = new List<fooditem>();
            try
            {
                bool isConnArgNull = (conn != null) ? false : true;
                MySqlConnection connection = (conn != null) ? conn : PrimaryConnection();
                //MySqlConnection connection = new MySqlConnection("server=132.148.68.125;user id = testusr; password=Testusr123!@#; database=testdatabase");
                tryOpenConnection(connection);
                string sql = "select * from fooditem ";
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
                                objfooditem = readerDatafooditem(reader);
                                lstfooditem.Add(objfooditem);

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

            return lstfooditem;


        }
        private static fooditem readerDatafooditem(MySqlDataReader reader)
        {
            fooditem objfooditem = new fooditem();
            objfooditem.ITEMID = Utility.IsValidInt(reader["ITEMID"]);
            objfooditem.ITEMNAME = Utility.IsValidString(reader["ITEMNAME"]);
            objfooditem.PRICE = Utility.IsValidInt(reader["PRICE"]);
            objfooditem.ITEMDESCRIPTION = Utility.IsValidString(reader["ITEMDESCRIPTION"]);
            objfooditem.ITEMCATEGORY = Utility.IsValidString(reader["ITEMCATEGORY"]);

            objfooditem.ITEMSTATUS = Utility.IsValidString(reader["ITEMSTATUS"]);
              

            return objfooditem;
        }
        public static string Savefooditem(fooditem objfooditem, MySqlConnection conn = null, MySqlTransaction trans = null)
        {
            string returnMessage = "";
            string sITEMID = "";
            sITEMID = objfooditem.ITEMID.ToString();
            var templstfooditem = Getfooditem("ITEMID='" + sITEMID + "'", conn);
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
                    if (templstfooditem.Count <= 0)
                    {
                        isEdit = false;
                        sql = @"INSERT INTO fooditem(
                                    ITEMNAME,PRICE,ITEMDESCRIPTION,ITEMSTATUS,ITEMCATEGORY) VALUES(@ITEMNAME,@PRICE,@ITEMDESCRIPTION,@ITEMSTATUS,@ITEMCATEGORY)";
                    }
                    else
                    {
                        sql = @"UPDATE fooditem set ITEMID=@ITEMID,ITEMNAME=@ITEMNAME, PRICE=@PRICE,ITEMDESCRIPTION=@ITEMDESCRIPTION,ITEMCATEGORY=@ITEMCATEGORY,ITEMSTATUS=@ITEMSTATUS WHERE ITEMID=@ITEMID";
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
                        command.Parameters.AddWithValue("@ITEMID", objfooditem.ITEMID);
                    }
                    command.Parameters.AddWithValue("@ITEMNAME", objfooditem.ITEMNAME);
                    command.Parameters.AddWithValue("@PRICE", objfooditem.PRICE);
                    command.Parameters.AddWithValue("@ITEMDESCRIPTION", objfooditem.ITEMDESCRIPTION);
                    //command.Parameters.AddWithValue("@CATEGORYID", objfooditem.CATEGORYID);
                    command.Parameters.AddWithValue("@ITEMCATEGORY", objfooditem.ITEMCATEGORY);
                    command.Parameters.AddWithValue("@ITEMSTATUS", objfooditem.ITEMSTATUS);
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

        public static string Deletefooditem(string sITEMID, MySqlConnection conn = null)
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
                    sql = @"DELETE from fooditem Where ITEMID = @ITEMID";
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@ITEMID", sITEMID);
                    int affectedRows = command.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        returnMessage = "OK";
                    }
                    else
                    {
                        returnMessage = "Unable to delete, Please contact ISD";
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