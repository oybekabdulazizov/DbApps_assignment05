﻿using Project01.DTOs.Requests;
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
                    DateTime currentDate = DateTime.Now;
                    command.CommandText = @"SELECT MAX(IdEnrollment) AS MaxId FROM Enrollment 
                                        WHERE Semester=1 AND IdStudy=@idStudy;";
                    command.Parameters.AddWithValue("idStudy", _idStudy);

                    var dataReader2 = command.ExecuteReader();
                    int latestEntry = 0;

                    if (dataReader2.Read())
                    {

                        var result = dataReader2["MaxId"].ToString();
                        if (!string.IsNullOrEmpty(result))
                        {
                            latestEntry = int.Parse(result);
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

                                var maxId = dataReader3["MaxId"].ToString();
                                if (!string.IsNullOrEmpty(maxId))
                                {
                                    latestEntry = int.Parse(maxId);
                                }

                                latestEntry++;
                                dataReader3.Close();
                                command.CommandText = @"INSERT INTO Enrollment VALUES (@idEnroll, @_semester, @_idStudy, @_startDate)";
                                command.Parameters.AddWithValue("idEnroll", latestEntry);
                                command.Parameters.AddWithValue("_semester", _semester);
                                command.Parameters.AddWithValue("_idStudy", _idStudy);
                                command.Parameters.AddWithValue("_startDate", currentDate);
                                command.ExecuteNonQuery();
                            }
                        }

                    }

                    // here, we check if newly entered index number is assigned to another student. 
                    // If a student already exists with the given index number, we return error
                    // If not, then with that index number, we insert a new student into Students table
                    command.CommandText = @"SELECT FirstName FROM Student WHERE IndexNumber=@idStudent;";
                    command.Parameters.AddWithValue("idStudent", request.IndexNumber);
                    using var dataReader4 = command.ExecuteReader();
                    if (!dataReader4.Read())
                    {
                        dataReader4.Close();
                        command.CommandText =
                            @"INSERT INTO Student VALUES (@id, @name, @surname, CONVERT(DATE, @dob, 103), @idE);";
                        command.Parameters.AddWithValue("@id", request.IndexNumber);
                        command.Parameters.AddWithValue("@name", request.FirstName);
                        command.Parameters.AddWithValue("@surname", request.LastName);
                        command.Parameters.AddWithValue("@dob", request.BirthDate);
                        command.Parameters.AddWithValue("@idE", latestEntry);

                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        // returning error if the given id is already assigned to another student
                        return null;
                    }

                    transaction.Commit();

                    // Done :)
                    response = new EnrollmentResponse();
                    response.FirstName = request.FirstName;
                    response.LastName = request.LastName;
                    response.Studies = request.Studies;
                    response.Semester = _semester;

                    return response;
                }
            }
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
