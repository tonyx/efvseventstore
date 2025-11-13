using System;
using System.Linq;
using System.Threading.Tasks;
using efexample.Application.Interfaces;
using efexample.Application.Services;
using efexample.Data;
using efexample.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace efexample.Tests.Application
{
    public class SchoolServiceTests : IClassFixture<TestFixture>
    {
        private readonly ISchoolService _schoolService;
        private readonly SchoolContext _context;

        public SchoolServiceTests(TestFixture fixture)
        {
            _context = fixture.Context;
            _schoolService = new SchoolService(_context);
            fixture.ResetDatabase();
        }

        [Fact]
        public async Task EnrollStudentInCourse_WhenWithinLimits_ShouldSucceed()
        {
            // Arrange
            var student = new Student { Name = "Test Student", MaxCoursesAllowed = 2 };
            var course = new Course { Name = "Test Course", MaxStudents = 30 };
            
            await _context.Students.AddAsync(student);
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            // Act
            await _schoolService.EnrollStudentInCourseAsync(student.Id, course.Id);

            // Assert
            var updatedStudent = await _context.Students
                .Include(s => s.EnrolledCourses)
                .FirstOrDefaultAsync(s => s.Id == student.Id);
                
            Assert.Single(updatedStudent.EnrolledCourses);
            Assert.Equal(course.Id, updatedStudent.EnrolledCourses.First().Id);
        }

        [Fact]
        public async Task EnrollStudentInCourse_WhenExceedsStudentLimit_ShouldThrow()
        {
            // Arrange
            var student = new Student { Name = "Test Student", MaxCoursesAllowed = 1 };
            var course1 = new Course { Name = "Course 1", MaxStudents = 30 };
            var course2 = new Course { Name = "Course 2", MaxStudents = 30 };
            
            await _context.Students.AddAsync(student);
            await _context.Courses.AddRangeAsync(course1, course2);
            await _context.SaveChangesAsync();

            // Act & Assert
            await _schoolService.EnrollStudentInCourseAsync(student.Id, course1.Id);
            
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _schoolService.EnrollStudentInCourseAsync(student.Id, course2.Id));
        }

        [Fact]
        public async Task EnrollStudentInCourse_WhenExceedsCourseLimit_ShouldThrow()
        {
            // Arrange
            var student1 = new Student { Name = "Student 1", MaxCoursesAllowed = 5 };
            var student2 = new Student { Name = "Student 2", MaxCoursesAllowed = 5 };
            var course = new Course { Name = "Test Course", MaxStudents = 1 };
            
            await _context.Students.AddRangeAsync(student1, student2);
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            // Act & Assert
            await _schoolService.EnrollStudentInCourseAsync(student1.Id, course.Id);
            
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _schoolService.EnrollStudentInCourseAsync(student2.Id, course.Id));
        }
    }
}
