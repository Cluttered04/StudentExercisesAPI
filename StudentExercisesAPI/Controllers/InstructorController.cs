using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using StudentExercises.Models;
using Microsoft.AspNetCore.Http;

//// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StudentExercisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private readonly IConfiguration _config;
        public InstructorController(IConfiguration config)
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

        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] int id, string include)
            //public async Task<IActionResult> Get(int? cohort, string orderBy, int limit)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Instructor.Id, Instructor.FirstName, Instructor.LastName, Instructor.SlackHandle, Instructor.CohortId, Cohort.Id AS IdOfCohort, Cohort.Name AS CohortName, Student.FirstName AS StudentName FROM Instructor JOIN Cohort ON Instructor.CohortId = Cohort.Id JOIN Student ON Student.CohortId = Cohort.Id";

                    Instructor instructor = null;
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Instructor> instructors = new List<Instructor>();

                    while (reader.Read())
                    {
                        instructor = new Instructor()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            InstructorCohort = new Cohort()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("IdOfCohort")),
                                Name = reader.GetString(reader.GetOrdinal("CohortName")),
                                Students = new List<Student>(),
                                Instructors = new List<Instructor>()
                            }
                            
                        };

                        if (!instructors.Any(x => x.Id == instructor.Id))
                        {
                            instructors.Add(instructor);
                        }
                        
                    }
                    reader.Close();
                    return Ok(instructors);

                }
                
            }

            }



        }
    }
