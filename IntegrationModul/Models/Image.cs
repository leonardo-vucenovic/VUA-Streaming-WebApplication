using System.ComponentModel.DataAnnotations;

namespace IntegrationModul.Models
{
    public partial class Image
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The content for image is required.")]
        public string Content { get; set; } = null!;
    }
}
