using System.ComponentModel;

namespace AdministrativeModul.ViewModels
{
    public class VMVideoAdd
    {
        public DateTime CreatedAt { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; } = null!;

        [DisplayName("Description")]
        public string? Description { get; set; }

        [DisplayName("Genre")]
        public int GenreId { get; set; }
        [DisplayName("Image")]
        public int ImageId { get; set; }

        public int TotalSeconds { get; set; }

        public string? StreamingUrl { get; set; }
    }
}
