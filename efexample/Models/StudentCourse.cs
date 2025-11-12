using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efexample.Models;

public class StudentCourse
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    
    // Navigation properties
    public virtual Student Student { get; set; } = null!;
    public virtual Course Course { get; set; } = null!;
    
    // Additional properties for the join table can be added here
    // For example: public DateTime EnrollmentDate { get; set; }
}
