using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SIMS.API.DTOs.Attendance
{
	public class TakeAttendanceRequestDto
	{
		[Required]
		public DateOnly AttendanceDate { get; set; }

		[Required]
		[MinLength(1)]
		public List<StudentAttendanceDto> Attendances { get; set; } = new();
	}
}