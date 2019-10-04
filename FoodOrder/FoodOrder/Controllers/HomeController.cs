using FoodOrder.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodOrder.Controllers
{
    public class HomeController : Controller
    {   //default
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    if (GetSessionUser() != null)
        //    {
        //        if (filterContext.ActionDescriptor.ActionName.Equals("login"))
        //        {
        //            filterContext.Result = RedirectToAction("Menu", "Home");
        //        }
        //    }
        //    else
        //    {
        //        if (!filterContext.ActionDescriptor.ActionName.Equals("Login") && !filterContext.ActionDescriptor.ActionName.Equals("SignUP"))
        //        {
        //            filterContext.Result = RedirectToAction("Login", "Home");
        //        }
        //    }
        //}

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public void SetCart(string id, int quantity)
        {
            fooditem objfooditem = new fooditem();
            List<fooditem> lstfooditem = fooditemManager.Getfooditem("ITEMID='" + id + "'", null);
            objfooditem.ITEMNAME = lstfooditem.First().ITEMNAME;
            objfooditem.PRICE = lstfooditem.First().PRICE;
            Cart objcart = new Cart();
            objcart.ITEMID = int.Parse(id);
            objcart.ITEMNAME = objfooditem.ITEMNAME;
            objcart.PRICE = objfooditem.PRICE;
            objcart.CUSTOMERID = GetSessionUser().CUSTID;
            objcart.QUANTITY = quantity;
            objcart.subtotal = objcart.QUANTITY * objcart.PRICE;
            SetSessionCart(objcart);

            //return "Success";

        }
        public ActionResult CHeckView(string paymentmethod)
        {
            testmodel obj = new testmodel();
            obj.paymentmethod = paymentmethod; 
            
            return View(obj);
            

        }

        
        public string check(string paymentmethod)
        {
            foodorder objOrder = new foodorder();
            objOrder.ORDERDATE = DateTime.Now;
            objOrder.CUSTID = GetSessionUser().CUSTID;
            objOrder.PAYMENTMETHOD = paymentmethod;
            int amount = 0;
            foreach (var item in GetSessionCart())
            {
                amount += item.subtotal;
            }
            objOrder.AMOUNT = amount;
            orderdetails objOrderDetails = new orderdetails();
            MySqlConnection conn = Shared.BaseManager.PrimaryConnection();
            conn.Open();
            var transaction = conn.BeginTransaction();
           
            string ret = foodorderManager.Savefoodorder(objOrder, conn, transaction);
            int orderid;
            if (!ret.Equals(Shared.Constants.MSG_ERR_DBSAVE.Text))
            {
                orderid = int.Parse(ret);
                foreach (var item in GetSessionCart())
                {
                    if (!ret.Equals(Shared.Constants.MSG_ERR_DBSAVE.Text))
                    {
              
                        objOrderDetails.ORDERID = orderid;
                        objOrderDetails.ITEMID = item.ITEMID;
                        objOrderDetails.QUANTITY = item.QUANTITY;
                        ret = orderdetailsManager.Saveorderdetails(objOrderDetails, conn, transaction);
                    }


                }
                if (paymentmethod == "Wallet")
                {
                    wallet objwallet = new wallet();
                    List<wallet> lstwallet = walletManager.Getwallet("CustomerID='" + GetSessionUser().CUSTID + "'");

                    if (lstwallet.First().BALANCE > amount)
                    {
                        objwallet.WALLETID = lstwallet.First().WALLETID;
                        objwallet.CUSTOMERID = GetSessionUser().CUSTID;
                        objwallet.BALANCE =lstwallet.First().BALANCE-amount;
                        ret = walletManager.Savewallet(objwallet, conn, transaction);
                    }
                    else
                    {
                        ret = "insufficient amount in wallet";
                        transaction.Rollback();
                        conn.Close();
                        conn.Dispose();
                        return ret;
                    }
                }
                else if (paymentmethod == "MasterCard")
                {

                }
                if (ret.Equals(Shared.Constants.MSG_OK_DBSAVE.Text))
                {
                    payment objpayment = new payment();
                    objpayment.ORDERID = orderid;
                    objpayment.PAYMENTDATE = DateTime.Now;
                    ret = paymentManager.Savepayment(objpayment, conn, transaction);

                }

                if (ret.Equals(Shared.Constants.MSG_OK_DBSAVE.Text))
                {
                    transaction.Commit();
                    conn.Close();
                    conn.Dispose();

                    return Shared.Constants.MSG_SUCCESS.Text;
                }

            }

            transaction.Rollback();
            conn.Close();
            conn.Dispose();
            return Shared.Constants.MSG_ERR_SERVER.Text;

            
        }
        public string Checkout(string paymentmethod)
        {
            foodorder objOrder = new foodorder();
            objOrder.ORDERDATE = DateTime.Now;
            objOrder.CUSTID = GetSessionUser().CUSTID;
            objOrder.PAYMENTMETHOD = paymentmethod;
            int amount=0;

          foreach(var item in GetSessionCart())
            {
                amount += item.subtotal;
            }
            objOrder.AMOUNT = amount;
            orderdetails objOrderDetails = new orderdetails();
            MySqlConnection conn = Shared.BaseManager.PrimaryConnection();
            conn.Open();
            var transaction = conn.BeginTransaction();
            string ret = foodorderManager.Savefoodorder(objOrder,conn,transaction);

            int orderid;
            if (!ret.Equals(Shared.Constants.MSG_ERR_DBSAVE.Text))
            {
                orderid = int.Parse(ret);
                foreach (var item in GetSessionCart())
                {
                    if (!ret.Equals(Shared.Constants.MSG_ERR_DBSAVE.Text))
                    {
                        objOrderDetails.ORDERID = orderid;
                        objOrderDetails.ITEMID = item.ITEMID;
                        objOrderDetails.QUANTITY = item.QUANTITY;
                        ret = orderdetailsManager.Saveorderdetails(objOrderDetails, conn, transaction);
                    }


                }
                if (paymentmethod == "Wallet")
                {
                    wallet objwallet = new wallet();
                    List<wallet> lstwallet = walletManager.Getwallet("CustomerID='" + GetSessionUser().CUSTID + "'");

                    if (lstwallet.First().BALANCE > amount)
                    {
                        objwallet.WALLETID = lstwallet.First().WALLETID;
                        objwallet.CUSTOMERID = GetSessionUser().CUSTID;
                        objwallet.BALANCE -= amount;
                        ret = walletManager.Savewallet(objwallet, conn, transaction);
                    }
                    else
                    {
                        ret = "insufficient amount in wallet";
                        transaction.Rollback();
                        conn.Close();
                        conn.Dispose();
                        return ret;
                    }
                }
                else if (paymentmethod == "MasterCard")
                {

                }
                if (ret.Equals(Shared.Constants.MSG_OK_DBSAVE.Text))
                {
                    payment objpayment = new payment();
                    objpayment.ORDERID = orderid;
                    objpayment.PAYMENTDATE = DateTime.Now;
                    ret = paymentManager.Savepayment(objpayment, conn, transaction);

                }
               
                if (ret.Equals(Shared.Constants.MSG_OK_DBSAVE.Text))
                {
                    transaction.Commit();
                    conn.Close();
                    conn.Dispose();
            
                    return Shared.Constants.MSG_SUCCESS.Text;
                }

            }

            transaction.Rollback();
            conn.Close();
            conn.Dispose();
            return Shared.Constants.MSG_ERR_SERVER.Text;

        }
       
        //template
        public ActionResult Menu(string id)
        {
            List<fooditem> lstfooditems = fooditemManager.Getfooditem("");
            if (!string.IsNullOrEmpty(id))
            {
                id = id.ToLower();
                lstfooditems = lstfooditems.FindAll(x => x.ITEMNAME.ToLower().Contains(id) ||
                x.ITEMDESCRIPTION.ToLower().Contains(id) ||
                x.PRICE.ToString().Contains(id) ||
                x.ITEMCATEGORY.ToLower().Contains(id)

                );

            }
            return PartialView(lstfooditems);

        }


        public ActionResult Login()
        {
            if (Session[Shared.Constants.SESSION_USER] != null)
            {
                return RedirectToAction("Menu");
            }
            return View();
        }
        [HttpPost]
        public string Login(Customer model)

        {
            string ret = Shared.Constants.MSG_ERR_NOUSEREXIST.Text;
            List<Customer> lstCustomers = CustomerManager.GetCustomer("EMAIL='" + model.EMAIL + "'", null);
            if (lstCustomers.Count > 0)
            {
                if (lstCustomers.First().PASSWORD.Equals(model.PASSWORD))
                {
                    SetSessionUser(lstCustomers.First());
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
            SetSessionUser(null);
            Session[Shared.Constants.SESSION_CART] = null;
            return RedirectToAction("Login", "Home");
        }
        public ActionResult MyProfile()
        {
            if (GetSessionUser()==null)
            {
                return RedirectToAction("Login", "Home");
            }
            List<Customer> lstCustomer = CustomerManager.GetCustomerWallet(" CUSTID='"+ GetSessionUser().CUSTID + "'", null);
            if (lstCustomer.Count == 0)
            {
                string ret = "No record found!";
                return View(ret);
            }
            return View(lstCustomer.First());
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public string ChangePassword(string OLDPASSWORD, string NEWPASSWORD)
        {

            List<Customer> lstCustomer = CustomerManager.GetCustomer("PASSWORD='" + OLDPASSWORD + "'", null);
            if(lstCustomer.Count <= 0)
            {
                return "Invalid Password";
            }
            Customer obj = new Customer();
            obj = GetSessionUser();
            obj.PASSWORD = NEWPASSWORD;
            string ret = CustomerManager.SaveCustomer(obj, null);
            if (ret.Equals(Shared.Constants.MSG_SUCCESS.Text))
            {
                SetSessionUser(null);
                Session[Shared.Constants.SESSION_CART] = null;
                return "success";

            }

            return ret ;
        }
        public ActionResult Cart()
        {
           
            return View(GetSessionCart());
        }
        public ActionResult SignUP()
        {
            if (Session[Shared.Constants.SESSION_USER] != null)
            {
                return RedirectToAction("Menu");
            }
            return View();
        }
        
        [HttpPost]
        public String SignUp(VMSignup model)
        {
            List<Customer> lstCustomer = CustomerManager.GetCustomer(" EMAIL = '" + model.Email + "' OR CUSTUSERNAME='"+model.Username+"'", null);
            if (lstCustomer.Count > 0)
            {
                return "An account already exists with the given Email/UserName";
            }


            Customer objcustomer = new Customer();
            objcustomer.EMAIL = model.Email;
            objcustomer.PASSWORD = model.Password;
            objcustomer.CUSTUSERNAME = model.Username;
            objcustomer.CUSTNAME = model.FullName;

            MySqlConnection conn = Shared.BaseManager.PrimaryConnection();
            // MySqlConnection conn = new MySqlConnection("server=132.148.68.125;user id=testusr; password=Testusr123!@#; database=testdatabase");

            conn.Open();
            var transaction = conn.BeginTransaction();
            string ret = CustomerManager.SaveCustomer(objcustomer, conn, transaction);
            if (!ret.Equals(Shared.Constants.MSG_ERR_DBSAVE.Text))
            {
                objcustomer.CUSTID = int.Parse(ret);
                wallet objwallet = new wallet();
                objwallet.CUSTOMERID = objcustomer.CUSTID;
                objwallet.BALANCE = 0;
                ret = walletManager.Savewallet(objwallet, conn, transaction);
                if (!ret.Equals(Shared.Constants.MSG_ERR_DBSAVE.Text)) { 
                transaction.Commit();
                conn.Close();
                conn.Dispose();

                string token = "success";
                //string _token = (string)getVerificationURL(objcustomer);

                return token /*sendEmail(userobj.EMAIL, _token)*/;
                }
            }
            transaction.Rollback();
            conn.Close();
            conn.Dispose();
            return Shared.Constants.MSG_ERR_SERVER.Text;
        }
        public ActionResult Checkout()
        {


            return View();
        }
        

        //customer own feedbacks
       public ActionResult Feedback()
        {
            if (GetSessionUser() == null)
            {
                return RedirectToAction("Login", "Home");
            }
            List<FeedBack> lstfeedback = FeedBackManager.GetFeedback("CUSTID='" + GetSessionUser().CUSTID + "'", null);
          
            return View(lstfeedback);
        }


        [HttpPost]
        public string Feedback(FeedBack obj)
        {
            string ret = "";
            FeedBack objfeedback = new FeedBack();
            objfeedback.RATING = obj.RATING;
            objfeedback.FBDATE= DateTime.Now;
            objfeedback.FBDESCRIPTION = obj.FBDESCRIPTION;
            objfeedback.CUSTID = GetSessionUser().CUSTID;
            ret = FeedBackManager.SaveFeedback(objfeedback,null);
            

            return ret;

        }

        //Sessions

        public Customer GetSessionUser()
        {
            if (Session[Shared.Constants.SESSION_USER] != null)
            {
                return Session[Shared.Constants.SESSION_USER] as Customer;
            }

            return null;
        }

        public void SetSessionUser(Customer obj)
        {
            Session[Shared.Constants.SESSION_USER] = obj;
        }


        public void SetSessionCart(Cart obj)
        {
            List<Cart> lst = GetSessionCart();
            lst.Add(obj);
            Session[Shared.Constants.SESSION_CART] = lst;
        }

        public List<Cart> GetSessionCart()
        {
            if (Session[Shared.Constants.SESSION_CART] != null)
            {
                return Session[Shared.Constants.SESSION_CART] as List<Cart>;
            }

            return new List<Cart>();

        }




        //misc
        public ActionResult View1()
        {
            return PartialView();
        }
        [HttpPost]
        public void View1(orderdetails obj)
        {
            string ret = orderdetailsManager.Saveorderdetails(obj);
            
        }

        public ActionResult CustomerOrder(string id = "1")
        {

            //Customer SessionUser = GetSessionUser();
            //if (SessionUser != null)
            {
                orderdetails obj;
                List<orderdetails> lstorderdetails = orderdetailsManager.GetOrderDetailsItem(/*SessionUser.objorder.ORDERID.ToString()*/id);
                obj = orderdetailsManager.getTotalAmount(id);
                int amount = obj.TOTAL;
                return View(lstorderdetails);


            }

            //List<jpopening> lstjpopeningapplications = jpopeningManager.Getjpopening("PROFILEID='" + id + "'");

            //return PartialView();
        }
        public ActionResult CustomerTotalAmount(string id = "1")
        {
            //foodorder obj=new foodorder();
            //obj.AMOUNT = foodorderManager.getTotalAmount(id);
            //obj = foodorderManager.GetOrderCustomer(id)>;
            List<foodorder> lstfoodorder = foodorderManager.GetOrderCustomer(id);
            return View(lstfoodorder);
        }


        public ActionResult Imagetest()
        {
            return View();


        }
        [HttpPost]
        public ActionResult Imagetest(imagetest objimage)
        {
            string fileName = Path.GetFileNameWithoutExtension(objimage.IMAGEFILE.FileName);
            string extension = Path.GetExtension(objimage.IMAGEFILE.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            objimage.itemimage = "~/ProductImage/" + fileName;
            fileName = Path.Combine(Server.MapPath("~/ProductImage/"), fileName);
            objimage.IMAGEFILE.SaveAs(fileName);
            string ret = imagetestmanager.SaveImage(objimage, null);

            return View();


        }


    }
}