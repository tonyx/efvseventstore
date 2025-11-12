using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efexample.Models;

public class Student
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    public int MaxCoursesAllowed { get; set; }
    
    // Navigation property for the many-to-many relationship
    public virtual ICollection<Course> EnrolledCourses { get; set; } = new List<Course>();
    
    // Navigation property for the join table
    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
}
