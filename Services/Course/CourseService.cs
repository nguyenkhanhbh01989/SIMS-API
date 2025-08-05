using SIMS.API.DTOs.Course;
using SIMS.API.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SIMS.API.Services.Course;

namespace SIMS.API.Services.Course
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUserRepository _userRepository;

        public CourseService(ICourseRepository courseRepository, IUserRepository userRepository)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
        }

        public async Task<CourseViewDto> CreateCourseAsync(CreateUpdateCourseDto courseDto)
        {
            if (courseDto.TeacherId.HasValue)
            {
                var teacher = await _userRepository.GetUserByIdAsync(courseDto.TeacherId.Value);
                if (teacher == null || teacher.Role != "Teacher")
                {
                    throw new ApplicationException("ID giáo viên không hợp lệ hoặc người dùng không phải là giáo viên.");
                }
            }

            var newCourse = new Models.Course
            {
                Name = courseDto.Name,
                Semester = courseDto.Semester,
                TeacherId = courseDto.TeacherId,
                StartDate = courseDto.StartDate, // Gán giá trị nullable
                EndDate = courseDto.EndDate,     // Gán giá trị nullable
                CreatedAt = DateTime.UtcNow
            };

            var createdCourse = await _courseRepository.CreateAsync(newCourse);
            var result = await _courseRepository.GetByIdAsync(createdCourse.Id);
            return MapCourseToViewDto(result!);
        }

        // ... (Các phương thức còn lại không thay đổi)
        public async Task<IEnumerable<CourseViewDto>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            return courses.Select(MapCourseToViewDto);
        }

        public async Task<CourseViewDto?> GetCourseByIdAsync(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            return course == null ? null : MapCourseToViewDto(course);
        }

        public async Task<CourseViewDto> UpdateCourseAsync(int id, CreateUpdateCourseDto courseDto)
        {
            var courseToUpdate = await _courseRepository.GetByIdAsync(id);
            if (courseToUpdate == null)
            {
                throw new ApplicationException("Không tìm thấy môn học để cập nhật.");
            }

            if (courseDto.TeacherId.HasValue)
            {
                var teacher = await _userRepository.GetUserByIdAsync(courseDto.TeacherId.Value);
                if (teacher == null || teacher.Role != "Teacher")
                {
                    throw new ApplicationException("ID giáo viên mới không hợp lệ hoặc người dùng không phải là giáo viên.");
                }
            }

            courseToUpdate.Name = courseDto.Name;
            courseToUpdate.Semester = courseDto.Semester;
            courseToUpdate.TeacherId = courseDto.TeacherId;
            courseToUpdate.StartDate = courseDto.StartDate;
            courseToUpdate.EndDate = courseDto.EndDate;

            var updatedCourse = await _courseRepository.UpdateAsync(courseToUpdate);
            return MapCourseToViewDto(updatedCourse);
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
            {
                return false;
            }

            await _courseRepository.DeleteAsync(id);
            return true;
        }

        private CourseViewDto MapCourseToViewDto(Models.Course course)
        {
            return new CourseViewDto
            {
                Id = course.Id,
                Name = course.Name,
                Semester = course.Semester,
                TeacherId = course.TeacherId,
                TeacherName = course.Teacher?.FullName,
                CreatedAt = course.CreatedAt,
                StartDate = course.StartDate,
                EndDate = course.EndDate
            };
        }
    }
}