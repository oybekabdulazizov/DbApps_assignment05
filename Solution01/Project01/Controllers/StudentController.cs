using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project01.Models;

namespace Project01.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetStudents()
        {
            List<Student> studentsList = new List<Student>();

            using (var connection = new SqlConnection(DbConnection.connectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT s.IndexNumber, s.Firstname, s.Lastname, s.BirthDate, st.Name, e.Semester
                                           FROM Student s
                                           JOIN Enrollment e ON e.IdEnrollment=s.IdEnrollment 
                                           JOIN Studies st ON st.IdStudy=e.IdStudy";
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {

                        var student = new Student
                        {
                            IdStudent = reader["IndexNumber"].ToString(),
                            FirstName = reader["Firstname"].ToString(),
                            LastName = reader["Lastname"].ToString(),
                            BirthDate = DateTime.Parse(reader["BirthDate"].ToString()),
                            Studies = reader["Name"].ToString(),
                            Semester = int.Parse(reader["Semester"].ToString())
                        };
                        studentsList.Add(student);
                    }
                }
            }

            return Ok(studentsList);
        }

    }
}
