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
    public class ComputersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ComputersController(IConfiguration config)
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
        // GET api/Computers
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"SELECT cmp.Id, cmp.PurchaseDate, cmp.DecomissionDate, cmp.Make, cmp.Manufacturer 
                                        FROM Computer cmp";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<Computer> computers = new List<Computer>();
                    while (reader.Read())
                    {
                        Computer computer = new Computer()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {
                            computer.DecomissonDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                        }
                        computers.Add(computer);
                    }
                    reader.Close();
                    return Ok(computers);
                }
            }
        }

        //// GET: api/Computers/1
        //[HttpGet("{id}", Name = "GetComputer")]
        //public async Task<IActionResult> Get(int id)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"SELECT cmp.Id, cmp.PurchaseDate, cmp.DecomissionDate, cmp.Make, cmp.Manufacturer 
        //                                FROM Computer cmp
        //                                WHERE cmp.Id = @id";
        //            cmd.Parameters.Add(new SqlParameter("@id", id));

        //            SqlDataReader reader = await cmd.ExecuteReaderAsync();

        //            Computer computer = null;
        //            if (reader.Read())
        //            {
        //                computer = new Computer
        //                {
        //                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                    PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
        //                    DecomissonDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
        //                    Make = reader.GetString(reader.GetOrdinal("Make")),
        //                    Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
        //                };

        //            }
        //            reader.Close();
        //            return Ok(computer);
        //        }
        //    }
        //}


        //// POST api/TrainingPrograms
        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody] TrainingProgram trainingProgram)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"
        //                               INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees)
        //                               OUTPUT INSERTED.Id
        //                               VALUES (@name, @startDate, @endDate, @maxAttendees)";
        //            cmd.Parameters.Add(new SqlParameter("@name", trainingProgram.Name));
        //            cmd.Parameters.Add(new SqlParameter("@startDate", trainingProgram.StartDate));
        //            cmd.Parameters.Add(new SqlParameter("@endDate", trainingProgram.EndDate));
        //            cmd.Parameters.Add(new SqlParameter("@maxAttendees", trainingProgram.MaxAttendees));

        //            trainingProgram.Id = (int)await cmd.ExecuteScalarAsync();

        //            return CreatedAtRoute("GetTrainingProgram", new { id = trainingProgram.Id }, trainingProgram);
        //        }
        //    }
        //}

        //// PUT api/TrainingPrograms/2
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Put([FromRoute]int id, [FromBody] TrainingProgram trainingProgram)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = Connection)
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"
        //                    UPDATE TrainingProgram
        //                    SET Name = @name,
        //                        StartDate = @startDate,
        //                        EndDate = @endDate,
        //                        MaxAttendees = @maxAttendees
        //                    WHERE Id = @id";
        //                cmd.Parameters.Add(new SqlParameter("@id", id));
        //                cmd.Parameters.Add(new SqlParameter("@name", trainingProgram.Name));
        //                cmd.Parameters.Add(new SqlParameter("@startDate", trainingProgram.StartDate));
        //                cmd.Parameters.Add(new SqlParameter("@endDate", trainingProgram.EndDate));
        //                cmd.Parameters.Add(new SqlParameter("@maxAttendees", trainingProgram.MaxAttendees));

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
        //        if (!TrainingProgramExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

        //// DELETE: api/TrainingPrograms/2
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete([FromRoute] int id)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = Connection)
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"DELETE FROM TrainingProgram WHERE Id = @id";
        //                cmd.Parameters.Add(new SqlParameter("@id", id));

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
        //        if (!TrainingProgramExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

        //private bool TrainingProgramExists(int id)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            // More string interpolation
        //            cmd.CommandText = "SELECT Id FROM TrainingProgram WHERE Id = @id";
        //            cmd.Parameters.Add(new SqlParameter("@id", id));

        //            SqlDataReader reader = cmd.ExecuteReader();

        //            return reader.Read();
        //        }
        //    }
        //}
    }
}
