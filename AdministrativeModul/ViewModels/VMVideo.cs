using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdministrativeModul.ViewModels
{
    public class VMVideo
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        [DisplayName("Video name")]
        [Required(ErrorMessage = "The video name is required.")]
        [StringLength(256)]
        public string Name { get; set; }


        [Required(ErrorMessage = "The description is required.")]
        [StringLength(1024)]
        public string Description { get; set; }
        [Required(ErrorMessage = "The genreID is required.")]
        public int GenreId { get; set; }
        [Required(ErrorMessage = "The totalSeconds is required.")]
        public int TotalSeconds { get; set; }
        [Required(ErrorMessage = "The streamingUrl is required.")]
        [StringLength(256)]
        public string StreamingUrl { get; set; }

        public int ImageId { get; set; }

        public virtual VMGenre Genre { get; set; }

        public virtual VMImage Image { get; set; }
        [DisplayName("Genre name")]
        public string GenreName { get; set; }
        public string ImageContent { get; set; }

        public virtual VMGenre VMGenre { get; set; } = null!;

        public virtual VMImage VMImage { get; set; }

        public virtual ICollection<VMVideoTag> VideoTags { get; set; } = new List<VMVideoTag>();

        public List<int> SelectedTagIds { get; set; }
    }
}
