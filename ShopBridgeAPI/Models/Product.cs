using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace ShopBridgeAPI.Models
{
    public class Product
    {        
        public long product_id { get; set; }
        public int category_id { get; set; }        
        public int sub_category_id { get; set; }        
        public string product_name { get; set; }
        public string product_desc { get; set; }
        public double unit_price { get; set; }
        public double sell_price { get; set; }
        public float discount { get; set; }
        public double discount_price { get; set; }
        public string product_img { get; set; }
        public int stock_qty { get; set; }
        public string created_by { get; set; }
        public string modified_by { get; set; }

        public Product()
        {
            product_desc = "";
            product_name = "";
            product_img ="";
            created_by = "";
            modified_by = "";
            product_id = 0;
            category_id = 0;
            sub_category_id = 0;
            stock_qty = 0;
            unit_price = 0;
            sell_price = 0.0;
            discount = 0;
            discount_price = 0.0;
            calculateSellPrice(1);
        }

        public void calculateSellPrice(int qty)
        {
            sell_price = (unit_price * qty) * ((discount * 1.0) / 100);
            discount_price = unit_price - sell_price;
        }


    }
}
