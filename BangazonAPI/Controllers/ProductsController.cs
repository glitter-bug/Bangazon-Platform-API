using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductsController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET api/Products
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    
                    cmd.CommandText = @"SELECT p.Id, p.ProductTypeId, p.CustomerId, p.Price, p.Title, p.Description, p.Quantity 
                    FROM Product p";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<Product> products = new List<Product>();
                    while (reader.Read())
                    {
                        Product product = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Price = reader.GetSqlMoney(reader.GetOrdinal("Price")).ToDouble(),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId"))
                        };
                        Customer customer = new Customer
                        {

                        };
                        ProductType productType = new ProductType
                        {

                        };
                        products.Add(product);
                    }
                    reader.Close();
                    return Ok(products);
                }
            }
        }
        // GET api/products/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> Get(int id)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"
        //                SELECT
        //                    Id, Price, Title, Description, Quantity, CustomerId, ProductTypeId
        //                FROM Products
        //                WHERE Id = @id";
        //            cmd.Parameters.Add(new SqlParameter("@id", id));

        //            SqlDataReader reader = await cmd.ExecuteReaderAsync();

        //            Product product = null;
        //            if (reader.Read())
        //            {
        //                product = new Product
        //                {
        //                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                    Price = reader.GetInt32(reader.GetOrdinal("Price")),
        //                    Title = reader.GetString(reader.GetOrdinal("Title")),
        //                    Description = reader.GetString(reader.GetOrdinal("Description")),
        //                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
        //                    CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
        //                    ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId"))
        //                };
        //            }
        //            reader.Close();
        //            return Ok(product);
        //        }
        //    }
        //}

        //// POST api/values
        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody] Product product)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            // More string interpolation
        //            cmd.CommandText = @"
        //                INSERT INTO Customer ()
        //                OUTPUT INSERTED.Id
        //                VALUES ()
        //            ";
        //            cmd.Parameters.Add(new SqlParameter("@firstName", product.FirstName));

        //            product.Id = (int)await cmd.ExecuteScalarAsync();

        //            return CreatedAtRoute("GetCustomer", new { id = customer.Id }, customer);
        //        }
        //    }
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Put(int id, [FromBody] Product product)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = Connection)
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"
        //                    UPDATE Customer
        //                    SET FirstName = @firstName
        //                    -- Set the remaining columns here
        //                    WHERE Id = @id
        //                ";
        //                cmd.Parameters.Add(new SqlParameter("@id", customer.Id));
        //                cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));

        //                int rowsAffected = await cmd.ExecuteNonQueryAsync();

        //                if (rowsAffected > 0)
        //                {
        //                    return new StatusCodeResult(StatusCodes.Status204NoContent);
        //                }

        //                throw new Exception("No rows affected");
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        if (!CustomerExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //}

        //private bool CustomerExists(int id)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            // More string interpolation
        //            cmd.CommandText = "SELECT Id FROM Customer WHERE Id = @id";
        //            cmd.Parameters.Add(new SqlParameter("@id", id));

        //            SqlDataReader reader = cmd.ExecuteReader();

        //            return reader.Read();
        //        }
        //    }
        //}
    }
}
