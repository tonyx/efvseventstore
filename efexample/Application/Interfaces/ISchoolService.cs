using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using efexample.Models;

namespace efexample.Application.Interfaces
{
    public interface ISchoolService
    {
        Task<Student> AddStudentAsync(Student student);
        Task<Course> AddCourseAsync(Course course);
        Task EnrollStudentInCourseAsync(Guid studentId, Guid courseId);
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<IEnumerable<Course>> GetAllCoursesAsync();
    }
}
