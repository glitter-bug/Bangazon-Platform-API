﻿using System;
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
    public class CustomersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CustomersController(IConfiguration config)
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

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get(string q, string active)
        {

            string SqlCommandText;

            if (q != null)
            {
                SqlCommandText = $@"SELECT c.Id as CustomerId, c.FirstName, c.LastName 
                FROM Customer c WHERE (
                c.FirstName LIKE @q
                OR c.LastName LIKE @q
                )";
            }
            else if (active == "false")
            {
                SqlCommandText = @"SELECT c.Id AS CustomerId, c.FirstName, c.LastName, o.Id AS OrderId FROM Customer c LEFT JOIN [Order] o ON c.Id = o.Id WHERE o.Id IS NULL";
            }
            else
            {
                SqlCommandText = @"SELECT c.Id as CustomerId, c.FirstName, c.LastName FROM Customer c";
            }


            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = SqlCommandText;


                    if (q != null)
                    {
                        cmd.Parameters.Add(new SqlParameter("@q", $"%{q}%"));
                    }

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<Customer> customers = new List<Customer>();

                    while (reader.Read())
                    {
                        Customer customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            // You might have more columns
                        };

                        customers.Add(customer);
                    }

                    reader.Close();

                    return Ok(customers);
                }
            }
        }

        // GET api/values/5
        [HttpGet("{id}", Name = "GetCustomer")]
        public async Task<IActionResult> Get([FromRoute] int id, string _include)
        {
            if (!CustomerExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }

            string SqlCommandText;

            if(_include == "products")
            {
                SqlCommandText = @"SELECT c.Id AS CustomerId, c.FirstName, c.LastName, 
                p.Id AS ProductId, p.Title, p.Price, p.Description, p.Quantity, p.ProductTypeId 
                FROM Customer c LEFT JOIN Product p ON c.Id = p.CustomerId";
            }
            else if(_include == "payments")
            {
                SqlCommandText = @"SELECT c.Id AS CustomerId, c.FirstName, c.LastName, 
                pt.Id AS PaymentTypeId, pt.AcctNumber, pt.Name, pt.CustomerId 
                FROM Customer c LEFT JOIN PaymentType pt ON c.Id = pt.CustomerId";
            }
            else
            {
                SqlCommandText = @"SELECT c.Id AS CustomerId, c.FirstName, c.LastName FROM Customer c";
            }

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"{SqlCommandText} WHERE c.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Customer customer = null;

                    while (reader.Read())
                    {
                        if (customer == null)
                        {
                            customer = new Customer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            };
                        }

                        if(_include == "products")
                        {
                            if(!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                            {
                                customer.Products.Add(
                                    new Product
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                        ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                        Title = reader.GetString(reader.GetOrdinal("Title")),
                                        Price = reader.GetSqlMoney(reader.GetOrdinal("Price")).ToDouble(),
                                        Description = reader.GetString(reader.GetOrdinal("Description")),
                                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                                    }
                                );
                            }
                        }

                        if(_include == "payments")
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")))
                                customer.PaymentTypes.Add(
                                    new PaymentType
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                        AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                                        Name = reader.GetString(reader.GetOrdinal("Name"))
                                    });
                                }
                            }


                    reader.Close();

                    return Ok(customer);
                }
            }
        }


        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"
                        INSERT INTO Customer (FirstName, LastName)
                        OUTPUT INSERTED.Id
                        VALUES (@firstName, @lastName)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", customer.LastName));

                    customer.Id = (int)await cmd.ExecuteScalarAsync();


                    return CreatedAtRoute("GetCustomer", new { id = customer.Id }, customer);
                }
            }
        }

        //PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Customer customer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Customer
                            SET FirstName = @firstName,
                                LastName = @lastName
                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", customer.LastName));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/values/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //}

        private bool CustomerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM Customer WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}
