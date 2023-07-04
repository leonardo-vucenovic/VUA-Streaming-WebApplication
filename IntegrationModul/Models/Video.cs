using BusinessLayer.DALModels;
using System.ComponentModel.DataAnnotations;

namespace IntegrationModul.Models
{
    public partial class Video
    {
        public int Id { get; } = 0;

        public DateTime CreatedAt { get; } = DateTime.Now;

        [Required(ErrorMessage = "The name for video is required.")]
        public string Name { get; set; }

        public string Description { get; set; }

        public int GenreId { get; set; }

        public int TotalSeconds { get; set; }

        public string StreamingUrl { get; set; }

        public int ImageId { get; set; }

        public virtual Genre Genre { get; set; } = null!;

        public virtual Image Image { get; set; }

        public virtual ICollection<VideoTag> VideoTags { get; set; } = new List<VideoTag>();
    }
}
