using System.ComponentModel.DataAnnotations;

namespace PublicModul.ViewModels
{
    public class VMNotification
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        [Required(ErrorMessage = "The receiver for email is required.")]
        [StringLength(256)]
        public string ReceiverEmail { get; set; } = null!;
        public string? Subject { get; set; }
        [Required(ErrorMessage = "The body for email is required.")]
        [StringLength(1023)]
        public string Body { get; set; }
        public DateTime SentAt { get; set; }
    }
}
