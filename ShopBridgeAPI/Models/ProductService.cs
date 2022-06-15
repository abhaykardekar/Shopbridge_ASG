using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ShopBridgeAPI.Models
{
    public class ProductService : IProductService
    {
        public void WriteDBLog(string errorcode, string method, string uniquekey, string errordesc, string uniquerefid, IConfiguration configuration)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string con = configuration.GetConnectionString("Conn");
                SqlConnection sqlcon = new SqlConnection(con);
                cmd.Connection = sqlcon;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_InsertAPILogs";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@method", method);
                cmd.Parameters.AddWithValue("@uniquekey", uniquekey);
                cmd.Parameters.AddWithValue("@errorcode", errorcode);
                cmd.Parameters.AddWithValue("@errordesc", errordesc);
                cmd.Parameters.AddWithValue("@uniquerefid", uniquerefid);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }
            catch (Exception e) { }
        }

        //Method used to SaveImage on Root folder
        public string SaveImage(string prodimg_Base64,IWebHostEnvironment env)
        {
            Random random = new Random();
            int seqno = random.Next(1000000);
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            if (!Directory.Exists(env.ContentRootPath + "/Photos/Products"))
            {
                Directory.CreateDirectory(env.ContentRootPath + "\\Photos\\Products");
            }

            string saveImgpath = env.ContentRootPath + "\\Photos\\Products\\" + seqno.ToString() + "_" + timestamp + ".jpg";
            try
            {
                byte[] imageBytes = Convert.FromBase64String(prodimg_Base64);
                if (imageBytes.LongLength > 0 && imageBytes.LongLength < 1048000)
                {
                    using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                        image.Save(saveImgpath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }

                saveImgpath = "\\Photos\\Products\\" + seqno.ToString() + "_" + timestamp + ".jpg";
                return saveImgpath;
            }
            catch (Exception er)
            {
                return "Error.png";
            }
        }

        public byte[] GetImage(string prodimg_path,IWebHostEnvironment env)
        {
            string ImageFilePath = env.ContentRootPath + prodimg_path;
            try
            {
                return System.IO.File.ReadAllBytes(ImageFilePath);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        ResponseDetails IProductService.ListProduct(IConfiguration configuration)
        {
            DataSet ds = new DataSet();
            ResponseDetails resp = new ResponseDetails();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string con = configuration.GetConnectionString("Conn");
                SqlConnection sqlcon = new SqlConnection(con);
                cmd.Connection = sqlcon;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_get_productdetails";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        resp.error_code = "E101";
                        resp.error_desc = "No Product in Inventory";
                        WriteDBLog(resp.error_code, "GetProductList", "", resp.error_desc, "",configuration);
                        return resp;
                    }
                    else
                    {
                        resp.error_code = "0";
                        resp.error_desc = "Success";
                        resp.Data = ds;
                        WriteDBLog(resp.error_code, "GetProductList", "", resp.error_desc, "", configuration);
                        return resp;
                    }
                }
                else
                {
                    resp.error_code = "E101";
                    resp.error_desc = "No Product in Inventory";
                    WriteDBLog(resp.error_code, "GetProductList", "", resp.error_desc, "", configuration);
                    return resp;
                }
            }
            catch (Exception e)
            {
                resp.error_code = "E102";
                resp.error_desc = e.ToString();
                WriteDBLog(resp.error_code, "GetProductList", "", resp.error_desc, "", configuration);
                return resp;
            }
        }

        ResponseDetails IProductService.AddProduct(Product product, IConfiguration configuration,IWebHostEnvironment env)
        {
            DataSet ds = new DataSet();
            ResponseDetails resp = new ResponseDetails();
            string product_img_path = "";
            try
            {
                //Input Field validations
                bool is_Valid = true;
                StringBuilder str = new StringBuilder("Input Field Validation. Mandatory fields/Invalid values - ");
                if (product.product_name == null || product.product_name == string.Empty)
                {
                    is_Valid = false;
                    str.Append("Product name");
                }

                if (product.unit_price <= 0)
                {
                    is_Valid = false;
                    str.Append(", Product unitprice");
                }
                if (product.sell_price < 0)
                {
                    is_Valid = false;
                    str.Append(", Product sell price");
                }
                if (product.discount < 0)
                {
                    is_Valid = false;
                    str.Append(", Discount");
                }
                if (product.category_id <= 0)
                {
                    is_Valid = false;
                    if (str.Length > 0)
                        str.Append(", Product category");
                    else
                        str.Append("Product category");
                }
                if (product.sub_category_id <= 0)
                {
                    is_Valid = false;
                    if (str.Length > 0)
                        str.Append(", Product sub-category");
                    else
                        str.Append("Product sub-category");
                }

                if (is_Valid == false)
                {
                    resp.error_code = "E101";
                    resp.error_desc = str.ToString();
                    WriteDBLog(resp.error_code, "AddProduct", product.created_by, resp.error_desc, "", configuration);
                    return resp;
                }

                if (product.product_img != null || product.product_img != string.Empty)
                {
                    //Saving image to Root folder path and then path to DB
                    product_img_path = SaveImage(product.product_img,env);
                }
                else
                {
                    product_img_path = "";
                }

                SqlCommand cmd = new SqlCommand();
                string con = configuration.GetConnectionString("Conn");
                SqlConnection sqlcon = new SqlConnection(con);
                cmd.Connection = sqlcon;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_add_productdetails";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@category_id", product.category_id);
                cmd.Parameters.AddWithValue("@sub_category_id", product.sub_category_id);
                cmd.Parameters.AddWithValue("@prod_name", product.product_name);
                cmd.Parameters.AddWithValue("@prod_desc", product.product_desc);
                cmd.Parameters.AddWithValue("@unit_price", product.unit_price);
                cmd.Parameters.AddWithValue("@sell_price", product.sell_price);
                cmd.Parameters.AddWithValue("@discount", product.discount);
                cmd.Parameters.AddWithValue("@discount_price", product.discount_price);
                cmd.Parameters.AddWithValue("@prod_img", product_img_path);
                cmd.Parameters.AddWithValue("@stock_qty", product.stock_qty);
                cmd.Parameters.AddWithValue("@createdby", product.created_by);
                cmd.Parameters.AddWithValue("@modifiedby", product.modified_by);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        resp.error_code = ds.Tables[0].Rows[0]["ErrorCode"].ToString();
                        resp.error_desc = ds.Tables[0].Rows[0]["ErrorDesc"].ToString();
                        WriteDBLog(resp.error_code, "AddProduct", product.created_by, resp.error_desc, "", configuration);
                        return resp;
                    }
                    else
                    {
                        resp.error_code = "E103";
                        resp.error_desc = "Failed to get product added status";
                        WriteDBLog(resp.error_code, "AddProduct", product.created_by, resp.error_desc, "", configuration);
                        return resp;
                    }
                }
                else
                {
                    resp.error_code = "E103";
                    resp.error_desc = "Failed to get product added status";
                    WriteDBLog(resp.error_code, "AddProduct", product.created_by, resp.error_desc, "", configuration);
                    return resp;
                }
            }
            catch (Exception e)
            {
                resp.error_code = "E102";
                resp.error_desc = e.ToString();
                WriteDBLog(resp.error_code, "AddProduct", product.created_by, resp.error_desc, "", configuration);
                return resp;
            }
        }

        ResponseDetails IProductService.DeleteProduct(Product product, IConfiguration configuration)
        {
            DataSet ds = new DataSet();
            ResponseDetails resp = new ResponseDetails();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string con = configuration.GetConnectionString("Conn");
                SqlConnection sqlcon = new SqlConnection(con);
                cmd.Connection = sqlcon;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_delete_productdetails";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue(
                    "@product_id", product.product_id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        resp.error_code = ds.Tables[0].Rows[0]["ErrorCode"].ToString();
                        resp.error_desc = ds.Tables[0].Rows[0]["ErrorDesc"].ToString();
                        WriteDBLog(resp.error_code, "DeleteProduct", product.modified_by, resp.error_desc, product.product_id.ToString(), configuration);
                        return resp;
                    }
                    else
                    {
                        resp.error_code = "E103";
                        resp.error_desc = "Failed to get product delete status";
                        WriteDBLog(resp.error_code, "DeleteProduct", product.modified_by, resp.error_desc, product.product_id.ToString(), configuration);
                        return resp;
                    }
                }
                else
                {
                    resp.error_code = "E103";
                    resp.error_desc = "Failed to get product delete status";
                    WriteDBLog(resp.error_code, "DeleteProduct", product.modified_by, resp.error_desc, product.product_id.ToString(), configuration);
                    return resp;
                }
            }
            catch (Exception e)
            {
                resp.error_code = "E102";
                resp.error_desc = e.ToString();
                WriteDBLog(resp.error_code, "DeleteProduct", product.modified_by, resp.error_desc, product.product_id.ToString(), configuration);
                return resp;
            }
        }

        ResponseDetails IProductService.ModifyProduct(Product product, IConfiguration configuration)
        {
            DataSet ds = new DataSet();
            ResponseDetails resp = new ResponseDetails();
            try
            {
                bool is_Valid = true;
                StringBuilder str = new StringBuilder("Input Field Validation. Mandatory Fields/Invalid values - ");
                if (product.product_id <= 0)
                {
                    is_Valid = false;
                    str.Append("Product id");
                }
                if (product.product_name == null || product.product_name == string.Empty)
                {
                    is_Valid = false;
                    str.Append("Product name");
                }
                if (product.unit_price <= 0)
                {
                    is_Valid = false;
                    str.Append(", Product unitprice");
                }
                if (product.sell_price < 0)
                {
                    is_Valid = false;
                    str.Append(", Product sell price");
                }
                if (product.discount < 0)
                {
                    is_Valid = false;
                    str.Append(", Discount");
                }

                if (is_Valid == false)
                {
                    resp.error_code = "E101";
                    resp.error_desc = str.ToString();
                    WriteDBLog(resp.error_code, "UpdateProduct", product.modified_by, resp.error_desc, product.product_id.ToString(), configuration);
                    return resp;
                }

                SqlCommand cmd = new SqlCommand();
                string con = configuration.GetConnectionString("Conn");
                SqlConnection sqlcon = new SqlConnection(con);
                cmd.Connection = sqlcon;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_update_productdetails";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@product_id", product.product_id);
                cmd.Parameters.AddWithValue("@prod_name", product.product_name);
                cmd.Parameters.AddWithValue("@prod_desc", product.product_desc);
                cmd.Parameters.AddWithValue("@unit_price", product.unit_price);
                cmd.Parameters.AddWithValue("@sell_price", product.sell_price);
                cmd.Parameters.AddWithValue("@discount", product.discount);
                cmd.Parameters.AddWithValue("@discount_price", product.discount_price);
                cmd.Parameters.AddWithValue("@stock_qty", product.stock_qty);
                cmd.Parameters.AddWithValue("@modifiedby", product.modified_by);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        resp.error_code = ds.Tables[0].Rows[0]["ErrorCode"].ToString();
                        resp.error_desc = ds.Tables[0].Rows[0]["ErrorDesc"].ToString();
                        WriteDBLog(resp.error_code, "UpdateProduct", product.modified_by, resp.error_desc, product.product_id.ToString(), configuration);
                        return resp;
                    }
                    else
                    {
                        resp.error_code = "E103";
                        resp.error_desc = "Failed to get product update status";
                        WriteDBLog(resp.error_code, "UpdateProduct", product.modified_by, resp.error_desc, product.product_id.ToString(), configuration);
                        return resp;
                    }
                }
                else
                {
                    resp.error_code = "E103";
                    resp.error_desc = "Failed to get product update status";
                    WriteDBLog(resp.error_code, "UpdateProduct", product.modified_by, resp.error_desc, product.product_id.ToString(), configuration);
                    return resp;
                }
            }
            catch (Exception e)
            {
                resp.error_code = "E102";
                resp.error_desc = e.ToString();
                WriteDBLog(resp.error_code, "UpdateProduct", product.modified_by, resp.error_desc, product.product_id.ToString(), configuration);
                return resp;
            }
        }

        ResponseDetails IProductService.UpdateProductImage(Product product,IConfiguration configuration,IWebHostEnvironment env)
        {
            DataSet ds = new DataSet();
            ResponseDetails resp = new ResponseDetails();
            string product_img_path = "";
            try
            {
                bool is_Valid = true;
                StringBuilder str = new StringBuilder("Input Field Validation. Invalid - ");

                if (product.product_id <= 0)
                {
                    is_Valid = false;
                    str.Append("Product Id");
                }

                if (product.modified_by == "")
                {
                    is_Valid = false;
                    str.Append("Modified by User Id");
                }

                if (is_Valid == false)
                {
                    resp.error_code = "E101";
                    resp.error_desc = str.ToString();
                    WriteDBLog(resp.error_code, "UpdateProductImage", product.created_by, resp.error_desc, product.product_id.ToString(), configuration);
                    return resp;
                }

                if (product.product_img != null || product.product_img != string.Empty)
                {
                    //Saving image to Root folder path and then path to DB
                    product_img_path = SaveImage(product.product_img,env);

                    SqlCommand cmd = new SqlCommand();
                    string con = configuration.GetConnectionString("Conn");
                    SqlConnection sqlcon = new SqlConnection(con);
                    cmd.Connection = sqlcon;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "usp_update_productImagedetails";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@product_id", product.product_id);
                    cmd.Parameters.AddWithValue("@prod_img", product_img_path);
                    cmd.Parameters.AddWithValue("@modifiedby", product.modified_by);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);

                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            resp.error_code = ds.Tables[0].Rows[0]["ErrorCode"].ToString();
                            resp.error_desc = ds.Tables[0].Rows[0]["ErrorDesc"].ToString();
                            WriteDBLog(resp.error_code, "UpdateProductImage", product.modified_by, resp.error_desc, product.product_id.ToString(), configuration);
                            return resp;
                        }
                        else
                        {
                            resp.error_code = "E103";
                            resp.error_desc = "Failed to get product image update status";
                            WriteDBLog(resp.error_code, "UpdateProductImage", product.modified_by, resp.error_desc, product.product_id.ToString(), configuration);
                            return resp;
                        }
                    }
                    else
                    {
                        resp.error_code = "E103";
                        resp.error_desc = "Failed to get product image update status";
                        WriteDBLog(resp.error_code, "UpdateProductImage", product.modified_by, resp.error_desc, product.product_id.ToString(), configuration);
                        return resp;
                    }
                }
                else
                {
                    product_img_path = "";
                    resp.error_code = "E104";
                    resp.error_desc = "No image exists!";
                    WriteDBLog(resp.error_code, "UpdateProductImage", product.modified_by, resp.error_desc, product.product_id.ToString(), configuration);
                    return resp;
                }
            }
            catch (Exception e)
            {
                resp.error_code = "E101";
                resp.error_desc = e.ToString();
                WriteDBLog(resp.error_code, "UpdateProductImage", product.modified_by, resp.error_desc, product.product_id.ToString(), configuration);
                return resp;
            }
        }

        ResponseDetails IProductService.GetProductCategories(IConfiguration configuration)
        {
            DataSet ds = new DataSet();
            ResponseDetails resp = new ResponseDetails();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string con = configuration.GetConnectionString("Conn");
                SqlConnection sqlcon = new SqlConnection(con);
                cmd.Connection = sqlcon;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_get_categorydetails";
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        resp.error_code = "0";
                        resp.error_desc = "Success";
                        resp.Data = ds;
                        WriteDBLog(resp.error_code, "GetProductCategories", "", resp.error_desc, "", configuration);
                        return resp;
                    }
                    else
                    {
                        resp.error_code = "E103";
                        resp.error_desc = "No Product Categories exists!";
                        WriteDBLog(resp.error_code, "GetProductCategories", "", resp.error_desc, "", configuration);
                        return resp;
                    }
                }
                else
                {
                    resp.error_code = "E103";
                    resp.error_desc = "Failed to get product categories";
                    WriteDBLog(resp.error_code, "GetProductCategories", "", resp.error_desc, "", configuration);
                    return resp;
                }
            }
            catch (Exception e)
            {
                resp.error_code = "E102";
                resp.error_desc = e.ToString();
                WriteDBLog(resp.error_code, "GetProductCategories", "", resp.error_desc, "", configuration);
                return resp;
            }
        }

        ResponseDetails IProductService.GetProductSubCategories(Product product, IConfiguration configuration)
        {
            DataSet ds = new DataSet();
            ResponseDetails resp = new ResponseDetails();
            try
            {
                SqlCommand cmd = new SqlCommand();
                string con = configuration.GetConnectionString("Conn");
                SqlConnection sqlcon = new SqlConnection(con);
                cmd.Connection = sqlcon;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_get_subcategorydetails";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@category_id", product.category_id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        resp.error_code = "0";
                        resp.error_desc = "Success";
                        resp.Data = ds;
                        WriteDBLog(resp.error_code, "GetProductSubCategories", "", resp.error_desc, "",configuration);
                        return resp;
                    }
                    else
                    {
                        resp.error_code = "E103";
                        resp.error_desc = "No Product Sub-Categories exists!";
                        WriteDBLog(resp.error_code, "GetProductSubCategories", "", resp.error_desc, "", configuration);
                        return resp;
                    }
                }
                else
                {
                    resp.error_code = "E103";
                    resp.error_desc = "Failed to get product categories";
                    WriteDBLog(resp.error_code, "GetProductCategories", "", resp.error_desc, "", configuration);
                    return resp;
                }
            }
            catch (Exception e)
            {
                resp.error_code = "E102";
                resp.error_desc = e.ToString();
                WriteDBLog(resp.error_code, "GetProductSubCategories", "", resp.error_desc, "", configuration);
                return resp;
            }
        }

        ResponseDetails IProductService.GetProductImage(Product product,IConfiguration configuration, IWebHostEnvironment env)
        {
            DataSet ds = new DataSet();
            ResponseDetails resp = new ResponseDetails();
            try
            {
                bool is_Valid = true;
                StringBuilder str = new StringBuilder("Input Field Validation. Invalid - ");

                if (product.product_id <= 0)
                {
                    is_Valid = false;
                    str.Append("Product Id");
                }

                if (is_Valid == false)
                {
                    resp.error_code = "E101";
                    resp.error_desc = str.ToString();
                    WriteDBLog(resp.error_code, "GetProductImage", product.created_by, resp.error_desc, product.product_id.ToString(), configuration);
                    return resp;
                }

                SqlCommand cmd = new SqlCommand();
                string con = configuration.GetConnectionString("Conn");
                SqlConnection sqlcon = new SqlConnection(con);
                cmd.Connection = sqlcon;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_get_productImagePath";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@product_id", product.product_id);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (ds.Tables[0].Rows[0]["prod_img_path"].ToString() == "")
                        {
                            resp.error_code = "E101";
                            resp.error_desc = "Failed to get product Image path";
                            WriteDBLog(resp.error_code, "GetProductImage", product.created_by, resp.error_desc, product.product_id.ToString(), configuration);
                            return resp;
                        }
                        else
                        {
                            resp.error_code = "0";
                            resp.error_desc = "Image retreived";
                            WriteDBLog(resp.error_code, "GetProductImage", product.created_by, resp.error_desc, product.product_id.ToString(), configuration);
                            byte[] file = GetImage(ds.Tables[0].Rows[0]["prod_img_path"].ToString(),env);
                            resp.filedata = file;
                            return resp;
                        }
                    }
                    else
                    {
                        resp.error_code = "E101";
                        resp.error_desc = "Failed to get product Image path";
                        WriteDBLog(resp.error_code, "GetProductImage", product.created_by, resp.error_desc, product.product_id.ToString(), configuration);
                        return resp;
                    }
                }
                else
                {
                    resp.error_code = "E101";
                    resp.error_desc = "Failed to get product Image path";
                    WriteDBLog(resp.error_code, "GetProductImage", product.created_by, resp.error_desc, product.product_id.ToString(), configuration);
                    return resp;
                }
            }
            catch (Exception e)
            {
                resp.error_code = "E101";
                resp.error_desc = e.ToString();
                WriteDBLog(resp.error_code, "GetProductImage", product.created_by, resp.error_desc, product.product_id.ToString(), configuration);
                return resp;
            }
        }
    }

    public interface IProductService
    {
        public ResponseDetails ListProduct(IConfiguration configuration);
        public ResponseDetails AddProduct(Product p, IConfiguration configuration,IWebHostEnvironment env);
        public ResponseDetails DeleteProduct(Product p, IConfiguration configuration);
        public ResponseDetails ModifyProduct(Product p, IConfiguration configuration);
        public ResponseDetails UpdateProductImage(Product p,IConfiguration configuration,IWebHostEnvironment env);
        public ResponseDetails GetProductCategories(IConfiguration configuration);
        public ResponseDetails GetProductSubCategories(Product p, IConfiguration configuration);
        public ResponseDetails GetProductImage(Product p,IConfiguration configuration, IWebHostEnvironment env);
    }
}
