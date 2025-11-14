using efexample.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace efexample.Tests;

public class StudentTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public StudentTests(TestFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetDatabase(); // Reset database before each test
    }

    [CollectionDefinition("Sequential")]
    public class SequentialCollectionFixture : ICollectionFixture<TestFixture>
    {
        // This class has no code, and is never created. Its purpose is simply to be the place to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
    }

    [Fact]
    public async Task AddStudent_ShouldSaveAndRetrieveStudent()
    {
        // Arrange
        var student = new Student
        {
            Name = "John Doe",
            MaxCoursesAllowed = 5
        };
        Console.WriteLine("Student: " + student);
        // Act - Add student
        _fixture.Context.Students.Add(student);
        await _fixture.Context.SaveChangesAsync();

        // Assert - Student was saved
        var savedStudent = await _fixture.Context.Students
            .FirstOrDefaultAsync(s => s.Name == "John Doe");
            
        Assert.NotNull(savedStudent);
        Assert.Equal("John Doe", savedStudent.Name);
        Assert.Equal(5, savedStudent.MaxCoursesAllowed);
        Assert.NotEqual(Guid.Empty, savedStudent.Id);
    }

    // fix asap
    [Fact(Skip = "Ignore this test")]
    public async Task AddStudent_WithCourses_ShouldSaveStudentWithCourses()
    {
        // Arrange
        var course = new Course
        {
            Name = "Computer Science 101",
            MaxStudents = 30
        };

        var student = new Student
        {
            Name = "Jane Smith",
            MaxCoursesAllowed = 5,
            EnrolledCourses = new List<Course> { course }
        };

        // Act
        _fixture.Context.Students.Add(student);
        await _fixture.Context.SaveChangesAsync();

        // Assert
        var savedStudent = await _fixture.Context.Students
            .Include(s => s.EnrolledCourses)
            .FirstOrDefaultAsync(s => s.Name == "Jane Smith");

        Assert.NotNull(savedStudent);
        Assert.Single(savedStudent.EnrolledCourses);
        Assert.Equal("Computer Science 101", savedStudent.EnrolledCourses.First().Name);
    }

    [Theory]
    [InlineData(1000)]  // Test with 1000 students
    [InlineData(5000)]  // Test with 5000 students
    [InlineData(10000)] // Test with 10000 students
    [InlineData(100000)] // Test with 100000 students
    public async Task BulkInsertStudents_PerformanceTest(int numberOfStudents)
    {
        // Arrange
        var random = new Random();
        var students = new List<Student>();
        
        // Generate random students
        for (int i = 0; i < numberOfStudents; i++)
        {
            students.Add(new Student
            {
                Name = $"Student_{i + 1}",
                MaxCoursesAllowed = random.Next(1, 6) // Random number between 1 and 5
            });
        }

        // Start timer
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // Act - Add all students in a single transaction
        await using var transaction = await _fixture.Context.Database.BeginTransactionAsync();
        try
        {
            await _fixture.Context.Students.AddRangeAsync(students);
            await _fixture.Context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        
        // Stop timer
        stopwatch.Stop();
        
        // Assert
        var actualCount = await _fixture.Context.Students.CountAsync();
        Assert.True(actualCount >= numberOfStudents, $"Expected at least {numberOfStudents} students, but found {actualCount}");
        
        // Output performance metrics
        var elapsedMs = stopwatch.ElapsedMilliseconds;
        var studentsPerSecond = numberOfStudents / (elapsedMs / 1000.0);
        
        Console.WriteLine($"Inserted {numberOfStudents} students in {elapsedMs}ms ({studentsPerSecond:F2} students/second)");
        
        // Optional: Add an assertion to fail if performance is below a certain threshold
        // For example, fail if we're inserting less than 100 students per second
        var minStudentsPerSecond = 100;
        Assert.True(studentsPerSecond >= minStudentsPerSecond, 
            $"Performance too slow: {studentsPerSecond:F2} students/second is below the threshold of {minStudentsPerSecond} students/second");
    }
}
