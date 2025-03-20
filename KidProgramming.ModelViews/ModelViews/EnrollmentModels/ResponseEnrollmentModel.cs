using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KidProgramming.ModelViews.ModelViews.EnrollmentModels
{
    public class ResponseEnrollmentModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public bool Status { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 100)]
        public decimal Progress { get; set; } = 0;
        public string? ThumbnailUrl { get; set; }

    }
}
