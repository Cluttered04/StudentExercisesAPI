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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StudentExercisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CohortController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CohortController(IConfiguration config)
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

        // GET: api/Coffee
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Cohort.Id, Name, Student.Id AS StudentId, Student.FirstName AS StudentFirstName, Student.LastName AS StudentLastName, Student.SlackHandle AS StudentSlackHandle, Student.CohortId AS StudentCohortId, Instructor.Id AS InstructorId, Instructor.FirstName as InstructorFirstName, Instructor.LastName AS InstructorLastName, Instructor.CohortId AS InstructorCohortId, Instructor.SlackHandle AS InstructorSlackHandle FROM Cohort JOIN Instructor ON Cohort.Id = Instructor.CohortId JOIN Student ON Student.CohortId = Cohort.Id";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Cohort> cohorts = new List<Cohort>();

                    while (reader.Read())
                    {
                        Cohort cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Students = new List<Student>(),
                            Instructors = new List<Instructor>()

                        };

                        Student student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                            FirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("StudentSlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("StudentCohortId"))
                        };

                        Instructor instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                            FirstName = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("InstructorLastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("InstructorSlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("InstructorCohortId"))
                        };

                        if(!cohorts.Any(x => x.Id == cohort.Id))
                        {
                            cohort.Instructors.Add(instructor);
                            cohort.Students.Add(student);
                            cohorts.Add(cohort);
                        }
                        else {
                            Cohort cohortToAdd = cohorts.Where(x => x.Id == cohort.Id).First();
                            if(!cohortToAdd.Instructors.Any(x => x.Id == instructor.Id))
                            {
                                cohortToAdd.Instructors.Add(instructor);
                            }
                            if(!cohortToAdd.Students.Any(x => x.Id == student.Id))
                            {
                                cohortToAdd.Students.Add(student);
                            }

                        };

                        
                    }
                    reader.Close();
                    return Ok(cohorts);
                }
            }
        }


        [HttpGet("{id}", Name = "GetCohort")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name FROM Cohort WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Cohort cohort = null;

                    if (reader.Read())
                    {
                        cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                    }
                    reader.Close();
                    return Ok(cohort);
                }
            }
        }



















    }
}


