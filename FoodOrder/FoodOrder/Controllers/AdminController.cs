using FoodOrder.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodOrder.Controllers
{
    public class AdminController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (GetSessionAdmin() != null)
            {
                if (filterContext.ActionDescriptor.ActionName.Equals("Signin"))
                {
                    filterContext.Result = RedirectToAction("AddProduct", "Admin");
                }
            }
            else
            {
                if (!filterContext.ActionDescriptor.ActionName.Equals("Signin"))
                {
                    filterContext.Result = RedirectToAction("Signin", "Admin");
                }
            }
        }
        // GET: Admin
        public ActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        public string Signin(admin model)
        {
            string ret = Shared.Constants.MSG_ERROR.Text;
            List<admin> lstADMIN = adminManager.Getadmin(" ADMINUSERNAME = '" + model.ADMINUSERNAME + "'", null); //and PASSWORD = '"+model.PASSWORD+"'
            if (lstADMIN.Count > 0)
            {
                if (lstADMIN.First().PASSWORD.Equals(model.PASSWORD))
                {
                    SetSessionAdmin(lstADMIN.First());
                    ret = Shared.Constants.MSG_SUCCESS.Text;
                }
                else
                {
                    ret = Shared.Constants.MSG_ERR_INVALIDCRED.Text;
                }
            }

            return ret;
        }

        public ActionResult Logout()
        {
            SetSessionAdmin(null);
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult ListItems()
        //{
        //    List<fooditem> lstfooditem = fooditemManager.Getfooditem(null);
        //    return PartialView(lstfooditem);
        //}

        public ActionResult Dashboard()
        {
            return PartialView();
        }


        public ActionResult ViewProduct(string id)
        {
                List<fooditem> lstfooditem = fooditemManager.Getfooditem("");
                    return  View(lstfooditem);
                
        }
        public ActionResult ViewFeedback(string id)
        {
            List<FeedBack> lstfeedback = FeedBackManager.GetFeedback("");
            return View(lstfeedback);

        }

        public ActionResult AddProduct(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                List<fooditem> lstfooditem =fooditemManager.Getfooditem(" ITEMID ='" +id+ "'");
                if (lstfooditem.Count > 0)
                {
                    return PartialView(lstfooditem.First());
                }

            }
            return PartialView(new fooditem());
        }
        public string _RemoveProduct(string id)
        {
            string ret = "";
            if (!string.IsNullOrEmpty(id))
            {
                List<fooditem> lstfooditem = fooditemManager.Getfooditem(" ITEMID ='" + id + "'");
                if (lstfooditem.Count > 0)
                {
                    ret = fooditemManager.Deletefooditem(id, null);
                    if (ret.Equals("OK"))
                    {
                        return ret; 
                    }
                }

            }
            return ret ;
        }


        [HttpPost]
        public string AddProduct(fooditem model)
        {
            //model. = DateTime.Now;
            string ret = fooditemManager.Savefooditem(model);
            if (ret.Equals(Shared.Constants.MSG_OK_DBSAVE.Text))
            {
                return Shared.Constants.MSG_SUCCESS.Text;
            }

            return Shared.Constants.MSG_ERROR.Text;
        }

        public ActionResult Deposit()
        {
            return View();
        }
        [HttpPost]
        public string DepositAmount(string id, int amount)
        {
            string ret ="";
            List<wallet> lstWallet = walletManager.Getwallet("WALLETID='" + id + "'", null);
          
            if (lstWallet.Count <= 0)
            {
                ret = "Incorrect Wallet ID";
            }

            transactionWallet objtrans = new transactionWallet();
            objtrans.CUSTID = lstWallet.First().CUSTOMERID;
            objtrans.TRANSACTIONDATE = DateTime.Now;
            objtrans.TRANSACTIONAMOUNT = amount;

            wallet objwallet = new wallet();
            objwallet.CUSTOMERID = lstWallet.First().CUSTOMERID;
            objwallet.WALLETID = int.Parse(id);
            objwallet.BALANCE = lstWallet.First().BALANCE + amount;
            MySqlConnection conn = Shared.BaseManager.PrimaryConnection();
            conn.Open();
            var transaction = conn.BeginTransaction();
            ret = transactionManager.Savetransaction(objtrans, conn, transaction);
            if (!ret.Equals(FoodOrder.Shared.Constants.MSG_ERR_DBSAVE.Text)){
                ret = walletManager.Savewallet(objwallet, null, transaction);
                if (ret.Equals(FoodOrder.Shared.Constants.MSG_OK_DBSAVE.Text))
                {

                    transaction.Commit();
                    conn.Close();
                    conn.Dispose();
                    ret = "Success";
                    return FoodOrder.Shared.Constants.MSG_SUCCESS.Text;
                }
            }
            transaction.Rollback();
            conn.Close();
            conn.Dispose();
            return Shared.Constants.MSG_ERR_SERVER.Text;
            

        }
        public admin GetSessionAdmin()
        {
            if (Session[Shared.Constants.SESSION_ADMIN] != null)
            {
                return Session[Shared.Constants.SESSION_ADMIN] as admin;
            }

            return null;
        }

        public void SetSessionAdmin(admin obj)
        {
            Session[Shared.Constants.SESSION_ADMIN] = obj;
        }


    }
}