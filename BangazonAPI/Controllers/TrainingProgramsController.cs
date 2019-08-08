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
    public class TrainingProgramsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TrainingProgramsController(IConfiguration config)
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

        // GET api/TrainingPrograms
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string completed)
        {
            // employee who sign up for a Training Program are included in the response
            string SqlCommandText = @"SELECT tp.Id, tp.Name, tp.StartDate, tp.EndDate, tp.MaxAttendees, 
                                            e.FirstName, e.LastName 
                                        FROM TrainingProgram tp
                                        JOIN EmployeeTraining et ON et.TrainingProgramId = tp.id
                                        JOIN Employee e ON e.Id = et.EmployeeId;";
            if (completed == "false")
            {
                // can view Training Programs only starting today or in the future
                SqlCommandText = @"SELECT tp.Id, tp.Name, tp.StartDate, tp.EndDate, tp.MaxAttendees, 
                                            e.FirstName, e.LastName FROM TrainingProgram tp
                                        JOIN EmployeeTraining et ON et.TrainingProgramId = tp.id
                                        JOIN Employee e ON e.Id = et.EmployeeId
                                        WHERE tp.EndDate > CURRENT_TIMESTAMP;";
            }
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = SqlCommandText;

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();
                    while (reader.Read())
                    {

                        int Id = reader.GetInt32(reader.GetOrdinal("Id"));
                        string Name = reader.GetString(reader.GetOrdinal("Name"));
                        DateTime StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate"));
                        DateTime EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"));
                        int MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"));
                        string FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                        string LastName = reader.GetString(reader.GetOrdinal("LastName"));

                        Employee employee = new Employee()
                        {
                            FirstName = FirstName,
                            LastName = LastName
                        };

                        if (!trainingPrograms.Any(train => train.Id == Id))
                        {
                            TrainingProgram trainingProgram = new TrainingProgram()
                            {
                                Id = Id,
                                Name = Name,
                                StartDate = StartDate,
                                EndDate = EndDate,
                                MaxAttendees = MaxAttendees,
                                AttendingEmployees = new List<Employee>()
                            };

                            trainingProgram.AttendingEmployees.Add(employee);
                            trainingPrograms.Add(trainingProgram);
                        }
                        else
                        {
                            var findProgram = trainingPrograms.Find(train => train.Id == Id);
                            findProgram.AttendingEmployees.Add(employee);
                        }
                    }

                    reader.Close();

                    return Ok(trainingPrograms);
                }
            }
        }

        // GET: api/TrainingPrograms/5
        [HttpGet("{id}", Name = "GetTrainingProgram")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT tp.Id, tp.Name, tp.StartDate, tp.EndDate, tp.MaxAttendees
                                        FROM TrainingProgram tp
                                        WHERE tp.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    TrainingProgram trainingProgram = null;
                    if (reader.Read())
                    {
                        trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };

                    }
                    reader.Close();
                    return Ok(trainingProgram);
                }
            }
        }
        // POST api/TrainingPrograms
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TrainingProgram trainingProgram)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                       INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees)
                                       OUTPUT INSERTED.Id
                                       VALUES (@name, @startDate, @endDate, @maxAttendees)";
                    cmd.Parameters.Add(new SqlParameter("@name", trainingProgram.Name));
                    cmd.Parameters.Add(new SqlParameter("@startDate", trainingProgram.StartDate));
                    cmd.Parameters.Add(new SqlParameter("@endDate", trainingProgram.EndDate));
                    cmd.Parameters.Add(new SqlParameter("@maxAttendees", trainingProgram.MaxAttendees));

                    trainingProgram.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetTrainingProgram", new { id = trainingProgram.Id }, trainingProgram);
                }
            }
        }

        // PUT api/TrainingPrograms/2
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute]int id, [FromBody] TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE TrainingProgram
                            SET Name = @name,
                                StartDate = @startDate,
                                EndDate = @endDate,
                                MaxAttendees = @maxAttendees
                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@name", trainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@startDate", trainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@endDate", trainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@maxAttendees", trainingProgram.MaxAttendees));

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
                if (!TrainingProgramExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE: api/TrainingPrograms/2
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT Id, Name, StartDate, EndDate, MaxAttendees 
                                        FROM TrainingProgram 
                                        WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        TrainingProgram trainingProgram = null;
                        if (reader.Read())
                        {
                            trainingProgram = new TrainingProgram
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                            };
                        }
                        reader.Close();
                        if (trainingProgram.StartDate > DateTime.Now)
                        {
                            using (SqlConnection secondConn = Connection)
                            {
                                secondConn.Open();
                                using (SqlCommand secondCmd = secondConn.CreateCommand())
                                {
                                    secondCmd.CommandText = @"DELETE FROM EmployeeTraining 
                                                         WHERE TrainingProgramId = @id
                                                         DELETE FROM TrainingProgram
                                                         WHERE Id = @id
                                                          ";
                                    // we have to delete from the table that holds the foreign key for Training Programs. We cant delete Training programs directly because there is a FK in use in EmployeeTraining Join Table.
                                    secondCmd.Parameters.Add(new SqlParameter("@id", id));

                                    int rowsAffected = secondCmd.ExecuteNonQuery();
                                    if (rowsAffected > 0)
                                    {
                                        return new StatusCodeResult(StatusCodes.Status204NoContent);
                                    }
                                    throw new Exception("No rows affected");
                                }
                            }
                        }
                        else
                        {
                            return StatusCode(403);
                        }
                    }
                }
            }
            catch (Exception)
            {
                if (!TrainingProgramExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        //A method to check if a training program existsxc
        private bool TrainingProgramExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM TrainingProgram WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
