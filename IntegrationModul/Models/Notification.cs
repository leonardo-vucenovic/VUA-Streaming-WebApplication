using System.ComponentModel.DataAnnotations;

namespace IntegrationModul.Models
{
    public partial class Notification
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        [Required(ErrorMessage = "The receiver for email is required.")]
        public string ReceiverEmail { get; set; } = null!;
        public string? Subject { get; set; }
        [Required(ErrorMessage = "The body for email is required.")]
        public string Body { get; set; }
        public DateTime SentAt { get; set; }
    }
}
