using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyApi.Controllers
{

    public class HomeController : Controller
    {

        private static Product[] products = { 
            new Product("P001","Acer",1000.0),
            new Product("P002","Asus",1500.0),
            new Product("P003","HP",1200.0),
            new Product("P004","Dell",900.0)
        };

        public IActionResult Index()
        {
            return View();
        }

         [Authorize]
         public IActionResult Secret()
         {
             return View();
         } 


       public IActionResult Authenticate()
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,"test_id")
            };

            // get secret key in bytes
            var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            // set symmetric security key 
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256; // Encryption algorithm

            var credentials = new SigningCredentials(key, algorithm);

            // Create JWT Token
            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audiance,
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(6),
                credentials
                );

            var jsonToken = new JwtSecurityTokenHandler().WriteToken(token);

            Token token1 = new Token();
            token1.token = jsonToken;
            return Ok(token1);
        }


        // GET API For Retrieve Secret Products
        [Authorize]
        [HttpGet]
        public Product[] SecretProducts()
        {
            return products;
        }

        // GET API For Retrieve Secret Products By ProductID
        [Authorize]
        [HttpGet]
        public Product GetProductByID([FromQuery] String productID)
        {
            foreach(Product product in products)
            {
                if (product.productID.ToLower().Equals(productID.ToLower())){
                    return product;
                }
            }
            return null;
        }
    }

}
