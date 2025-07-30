using SIMS.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIMS.API.Repositories.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<IEnumerable<Attendance>> GetAttendanceForCourseAsync(int courseId, DateOnly? date);
        Task UpsertAttendanceAsync(IEnumerable<Attendance> attendanceRecords);
    }
}