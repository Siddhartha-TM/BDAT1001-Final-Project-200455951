using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApi
{
    public class Product
    {
        public string productID { get; set; }
        public string productName { get; set; }
        public Double productPrice { get; set; }


        public Product(string productID,string productName,Double productPrice)
        {
            this.productID = productID;
            this.productName = productName;
            this.productPrice = productPrice;
        }
        
       
    }
}
