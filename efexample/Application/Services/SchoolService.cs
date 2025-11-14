using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using efexample.Application.Interfaces;
using efexample.Data;
using efexample.Models;
using Microsoft.EntityFrameworkCore;

namespace efexample.Application.Services
{
    public class SchoolService : ISchoolService
    {
        private readonly SchoolContext _context;

        public SchoolService(SchoolContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Student> AddStudentAsync(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));
            
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<Course> AddCourseAsync(Course course)
        {
            if (course == null) throw new ArgumentNullException(nameof(course));
            
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task EnrollStudentInCourseAsync(Guid studentId, Guid courseId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var student = await _context.Students
                    .Include(s => s.EnrolledCourses)
                    .FirstOrDefaultAsync(s => s.Id == studentId);

                if (student == null)
                    throw new InvalidOperationException("Student not found");

                var course = await _context.Courses
                    .Include(c => c.EnrolledStudents)
                    .FirstOrDefaultAsync(c => c.Id == courseId);

                if (course == null)
                    throw new InvalidOperationException("Course not found");

                if (student.EnrolledCourses.Any(c => c.Id == courseId))
                    return;
                
                if (course.EnrolledStudents.Count >= course.MaxStudents)
                    throw new InvalidOperationException("Course is full");

                if (student.EnrolledCourses.Count >= student.MaxCoursesAllowed)
                    throw new InvalidOperationException("Student has reached the maximum number of courses");

                student.EnrolledCourses.Add(course);
                course.EnrolledStudents.Add(student);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _context.Students
                .Include(s => s.EnrolledCourses)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Include(c => c.EnrolledStudents)
                .ToListAsync();
        }
    }
}
