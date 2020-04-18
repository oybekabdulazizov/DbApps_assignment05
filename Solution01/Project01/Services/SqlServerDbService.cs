using Project01.DTOs.Requests;
using Project01.DTOs.Responses;
using Project01.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace Project01.Services
{
    public class SqlServerDbService : IStudentDbService
    {

        public EnrollmentResponse EnrollStudents(EnrollmentRequest request)
        {

            int _semester = 1;
            EnrollmentResponse response;

            using (var connection = new SqlConnection(DbConnection.connectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    connection.Open();
                    var transaction = connection.BeginTransaction();
                    command.Transaction = transaction;

                    // let's check if all the passed values are valid 
                    if (request.IndexNumber.ToString() == null || request.FirstName.ToString() == null ||
                        request.LastName.ToString() == null || request.BirthDate == null || request.Studies.ToString() == null)
                    {
                        return null;
                    }

                    // let's check if the requested studies exist in the Studies table 
                    command.CommandText = @"SELECT IdStudy FROM Studies WHERE Name=@study;";
                    command.Parameters.AddWithValue("study", request.Studies);

                    var dataReader1 = command.ExecuteReader();
                    if (!dataReader1.Read())
                    {
                        return null;
                    }
                    int _idStudy = int.Parse(dataReader1["IdStudy"].ToString());
                    dataReader1.Close();

                    // for the existing study, let's find an entry with semester=1 
                    DateTime currentDate = DateTime.UtcNow;
                    command.CommandText = @"SELECT MAX(IdEnrollment) AS MaxId FROM Enrollment 
                                        WHERE Semester=1 AND IdStudy=@idStudy;";
                    command.Parameters.AddWithValue("idStudy", _idStudy);

                    var dataReader2 = command.ExecuteReader();
                    int latestEntry = 0;

                    if (dataReader2.Read())
                    {
                        latestEntry = int.Parse(dataReader2["MaxId"].ToString());
                        dataReader2.Close();
                    }
                    else
                    {
                        dataReader2.Close();
                        command.CommandText = @"SELECT MAX(IdEnrollment) AS MaxId FROM Enrollment;";
                        var dataReader3 = command.ExecuteReader();
                        // let's check if there any enrollment exists 
                        if (dataReader3.Read())
                        {
                            latestEntry = int.Parse(dataReader3["MaxId"].ToString());
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
                    command.CommandText = @"SELECT FirstName FROM Student WHERE IndexNumber=@indexNumber;";
                    command.Parameters.AddWithValue("indexNumber", request.IndexNumber);
                    using var dataReader4 = command.ExecuteReader();
                    if (!dataReader4.Read())
                    {
                        dataReader4.Close();
                        command.CommandText =
                            @"INSERT INTO Student VALUES (@idStudent, @firstName, @lastName, CONVERT(DATE, @birthdate, 103), @idEnroll);";
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
                        return null;
                    }

                    transaction.Commit();
                    response = new EnrollmentResponse
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Studies = request.Studies,
                        Semester = _semester
                    };
                }
            }
            // Done :) 
            return response;
        }



        public PromotionResponse PromoteStudents(PromotionRequest request)
        {
            PromotionResponse response;

            using (var connection = new SqlConnection(DbConnection.connectionString))
            {
                using (var command = new SqlCommand())
                {

                    command.Connection = connection;

                    command.CommandText = "PromoteStudents";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@studies", request.Studies);
                    command.Parameters.AddWithValue("@semester", request.Semester);
                    connection.Open();
                    command.ExecuteNonQuery();

                    response = new PromotionResponse
                    {
                        Studies = request.Studies,
                        Semester = request.Semester + 1
                    };

                }
            }

            return response;
        }
    }
}
