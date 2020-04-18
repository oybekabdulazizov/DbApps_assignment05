using Project01.DTOs.Requests;
using Project01.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project01.Services
{
    public interface IStudentDbService
    {
        public EnrollmentResponse EnrollStudents(EnrollmentRequest request);
        public PromotionResponse PromoteStudents(PromotionRequest request);
    }
}
