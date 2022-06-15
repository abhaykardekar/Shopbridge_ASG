using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using ShopBridgeAPI.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text;

namespace ShopBridgeAPI.Controllers
{    
    [Route("api/[controller]")]
    [ApiController]    
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private IProductService _productService;

        
        public ProductController(IConfiguration config, IWebHostEnvironment env)
        {
            _configuration = config;
            _env = env;
            _productService = new ProductService();
        }
                
        [HttpGet]
        public async Task<ResponseDetails> GetProductList()
        {
            await Task.Delay(100);
            return _productService.ListProduct(_configuration);
        }

        [Route("AddProduct")]
        [HttpPost]
        public async Task<ResponseDetails> AddProduct(Product product)
        {
            await Task.Delay(100);
            return _productService.AddProduct(product, _configuration,_env);
        }

        [Route("UpdateProduct")]
        [HttpPost]
        public async Task<ResponseDetails> UpdateProduct(Product product)
        {
            await Task.Delay(100);
            return _productService.ModifyProduct(product, _configuration);
        }

        [Route("DeleteProduct")]
        [HttpPost]
        public async Task<ResponseDetails> DeleteProduct(Product product)
        {
            await Task.Delay(100);
            return _productService.DeleteProduct(product, _configuration);
        }

        //Get Product Image on ID
        [Route("GetProductImage")]        
        [HttpPost]
        public async Task<ActionResult> GetProductImage(Product product)
        {
            await Task.Delay(100);
            ResponseDetails resp = new ResponseDetails();
            resp = _productService.GetProductImage(product, _configuration, _env);
            return resp.filedata==null ? Content(resp.error_desc) : File(resp.filedata,"");
        }

        [Route("UpdateProductImage")]
        [HttpPost]
        public async Task<ResponseDetails> UpdateProductImage(Product product)
        {
            await Task.Delay(100);
            return _productService.UpdateProductImage(product, _configuration, _env);
        }

        //Get Product Categories
        [Route("GetProductCategories")]
        [HttpPost]
        public async Task<ResponseDetails> GetProductCategories()
        {
            await Task.Delay(100);
            return _productService.GetProductCategories(_configuration);
        }

        //Get Product Categories
        [Route("GetProductSubCategories")]
        [HttpPost]
        public async Task<ResponseDetails> GetProductSubCategories(Product product)
        {
            await Task.Delay(100);
            return _productService.GetProductSubCategories(product, _configuration);
        }
    }
}
