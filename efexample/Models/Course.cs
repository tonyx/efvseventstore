using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efexample.Models;

public class Course
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    public int MaxStudents { get; set; }
    
    // Navigation property for the many-to-many relationship
    public virtual ICollection<Student> EnrolledStudents { get; set; } = new List<Student>();
    
    // Navigation property for the join table
    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
}
