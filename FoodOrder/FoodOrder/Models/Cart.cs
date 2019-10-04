using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodOrder.Models
{
    public class Cart
    {
        public int ITEMID { get; set; }
        public string ITEMNAME { get; set; }
        public int PRICE { get; set; }
        public int QUANTITY { get; set; }
        public int CUSTOMERID { get; set; }


        //other vars
        public int subtotal { get; set; }
        public int TOTALAMOUNT;

    }
}