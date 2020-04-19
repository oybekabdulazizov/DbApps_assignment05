using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project01.DTOs.Requests;
using Project01.Services;

namespace Project01.Controllers
{
    [Route("api")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {

        private readonly IStudentDbService _idbService;

        public EnrollmentController(IStudentDbService idbService)
        {
            _idbService = idbService;
        }

        [HttpPost("enrollstudent")]
        public IActionResult EnrollStudent(EnrollmentRequest request) 
        {

            var result = _idbService.EnrollStudents(request);
            if (result == null)
            {
                return BadRequest("Something went wrong!");
            }
            
            return Ok(result);

            /*int _semester = 1;
            EnrollStudentRes response;
            using (var connection = new SqlConnection(DbConnection.connectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    connection.Open();
                    var transaction = connection.BeginTransaction(System.Data.IsolationLevel.Serializable);
                    command.Transaction = transaction;

                    // let's check if all the passed values are valid 
                    if (request.IndexNumber.ToString() == null || request.FirstName.ToString() == null ||
                        request.LastName.ToString() == null || request.BirthDate == null || request.Studies.ToString() == null)
                    {
                        return BadRequest("You failed to provide all the required values.");
                    }

                    // let's check if the requested studies exist in the Studies table 
                    command.CommandText = @"SELECT IdStudy FROM Studies WHERE Name=@study;";
                    command.Parameters.AddWithValue("study", request.Studies);

                    var dataReader1 = command.ExecuteReader();
                    if (!dataReader1.Read())
                    {
                        return BadRequest("This study field does not exist.");
                    }
                    int _idStudy = int.Parse(dataReader1["IdStudy"].ToString());
                    dataReader1.Close();

                    // for the existing study, let's find the latest entry with semester=1 and check if it is not in the past
                    DateTime currentDate = DateTime.UtcNow;
                    command.CommandText = @"SELECT MAX(IdEnrollment) as maxEntry FROM Enrollment 
                                        WHERE Semester=1 AND IdStudy=@idStudy;";
                    command.Parameters.AddWithValue("idStudy", _idStudy);
                    var dataReader2 = command.ExecuteReader();
                    int latestEntry = 0;
                    if (dataReader2.Read())
                    {
                        latestEntry = int.Parse(dataReader2["maxEntry"].ToString());
                        dataReader2.Close();
                    }
                    else
                    {
                        dataReader2.Close();
                        command.CommandText = @"SELECT MAX(IdEnrollment) AS LatestEntry FROM Enrollment;";
                        var dataReader3 = command.ExecuteReader();
                        // let's check if there any enrollment exists 
                        if (dataReader3.Read())
                        {
                            latestEntry = int.Parse(dataReader3["LatestEntry"].ToString());
                        }

                        // adding a new enrollment
                        latestEntry++;
                        dataReader3.Close();
                        command.CommandText = @"INSERT INTO Enrollment VALUES (@idEnrollment, @semester, @idStudy, @startDate)";
                        command.Parameters.AddWithValue("idEnrollment", latestEntry);
                        command.Parameters.AddWithValue("semester", _semester);
                        command.Parameters.AddWithValue("idStudy", _idStudy);
                        command.Parameters.AddWithValue("startDate", currentDate);
                        command.ExecuteNonQuery();
                    }

                    // here, we check if newly entered index number is assigned to another student. 
                    // If a student already exists with the given index number, we return error
                    // If not, then with that index number, we insert a new student into Students table
                    command.CommandText = @"SELECT FirstName FROM Students WHERE IndexNumber=@indexNumber;";
                    command.Parameters.AddWithValue("indexNumber", request.IndexNumber);
                    var dataReader4 = command.ExecuteReader();
                    if (!dataReader4.Read())
                    {
                        dataReader4.Close();
                        command.CommandText =
                            @"INSERT INTO Students VALUES (@idStudent, @firstName, @lastName, CONVERT(DATE, @birthdate, 103), @idEnroll);";
                        command.Parameters.AddWithValue("idStudent", request.IndexNumber);
                        command.Parameters.AddWithValue("firstName", request.FirstName);
                        command.Parameters.AddWithValue("lastName", request.LastName);
                        command.Parameters.AddWithValue("birthdate", request.BirthDate);
                        command.Parameters.AddWithValue("idEnroll", latestEntry);

                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        // returning error if the given id is already assigned to another student
                        return BadRequest("Illegal action: Duplicate Id value entered");
                    }

                    transaction.Commit();
                    response = new EnrollStudentRes
                    {
                        IdStudent = request.IndexNumber,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        idEnrollment = latestEntry
                    };
                }

                // Done :) Let's just return a successful return code 
                return Ok($"Student {response.FirstName} has been accepted to semester {_semester} of {request.Studies} study!");
            }*/

        }
        /*
         * {
	"IndexNumber": "s128",
	"FirstName": "Elizabeth",
	"LastName": "Olsen",
	"BirthDate": "1995-05-12T00:00:00",
	"Studies": "Media Art"
}
         * 
         * 
         * 
         * 
         * 
        */
        [HttpPost("promotions")]
        public IActionResult PromoteStudent(PromotionRequest request)
        {
            return Ok(_idbService.PromoteStudents(request));


            /* PromoteStudentRes response;
             using (var connection = new SqlConnection(DbConnection.connectionString))
             {
                 using (var command = new SqlCommand())9u
                 {

                     command.Connection = connection;
                     // var transaction = connection.BeginTransaction();
                     // command.Transaction = transaction;

                     command.CommandText = "PromoteStudents";
                     command.CommandType = CommandType.StoredProcedure;
                     command.Parameters.AddWithValue("@studies", request.Studies);
                     command.Parameters.AddWithValue("@semester", request.Semester);
                     connection.Open();
                     command.ExecuteNonQuery();

                     response = new PromoteStudentRes
                     {
                         Studies = request.Studies,
                         Semester = request.Semester + 1
                     };

                     *//*command.CommandText = @"SELECT IdStudy FROM Studies WHERE Name=@study;";
                     command.Parameters.AddWithValue("study", request.Studies);
                     var dataReader1 = command.ExecuteReader();
                     if (!dataReader1.Read()) 
                     {
                         return NotFound("This study field does not exist.");
                     }
                     int _idStudy = int.Parse(dataReader1["IdStudy"].ToString());

                     command.CommandText = @"SELECT IdEnrollment FROM Enrollment WHERE Semester=@semester AND IdStudy=@idStudy;";
                     command.Parameters.AddWithValue("semester", request.Semester);
                     command.Parameters.AddWithValue("idStudy", _idStudy);
                     var dataReader2 = command.ExecuteReader();
                     if (!dataReader2.Read())
                     {
                         return NotFound($"Requested study with semester {request.Semester} does not exist!");
                     }
                     int _idEnrollment = int.Parse(dataReader2["IdEnrollment"].ToString());*//*
                 }
             }

             return Ok($"Students of {response.Studies} studies have been promoted to semester {response.Semester}!");*/
    }
}
}
