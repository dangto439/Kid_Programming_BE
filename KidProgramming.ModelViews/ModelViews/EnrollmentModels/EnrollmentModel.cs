using KidPrograming.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KidProgramming.ModelViews.ModelViews.EnrollmentModels
{
    public class EnrollmentModel
    {
         
        public string CourseId { get; set; }
        public string Title { get; set; }
        public Enrollment Enrollment { get; set; }
        public bool Status { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100)]
        public decimal Progress { get; set; } = 0;
        [Column(TypeName = "decimal(19,0)")]
        public decimal? Price { get; set; }
        public DateTimeOffset? PaymentDate { get; set; }
        public string? ThumbnailUrl { get; set; }

    
}
}
