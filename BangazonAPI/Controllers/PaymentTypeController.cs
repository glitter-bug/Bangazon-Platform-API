using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllersgv
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTypesController : ControllerBase
    {

        private readonly IConfiguration _config;

        public PaymentTypesController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: api/PaymentType
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, AcctNumber, Name, CustomerId
                    FROM PaymentType ";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<PaymentType> paymentTypes = new List<PaymentType>();
                    while (reader.Read())
                    {
                        PaymentType paymenttype = new PaymentType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            // You might have more columns
                        };

                        paymentTypes.Add(paymenttype);
                    }

                    reader.Close();

                    return Ok(paymentTypes);
                }
            }
        }

        //// GET: api/PaymentType/5
        [HttpGet("{id}", Name = "GetPaymentType")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, AcctNumber, Name, CustomerId
                    FROM PaymentType 
                    WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    PaymentType paymenttype = null;

                    if (reader.Read())
                    {
                        paymenttype = new PaymentType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            // You might have more columns
                        };




                        reader.Close();

                        return Ok(paymenttype);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            //// POST: api/PaymentType
            //[HttpPost]
            //public void Post([FromBody] string value)
            //{
            //}

            //// PUT: api/PaymentType/5
            //[HttpPut("{id}")]
            //public void Put(int id, [FromBody] string value)
            //{
            //}

            //// DELETE: api/ApiWithActions/5
            //[HttpDelete("{id}")]
            //public void Delete(int id)
            //{
            //}
        }
    }
}
