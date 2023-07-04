using BusinessLayer.BLModels;
using System.ComponentModel.DataAnnotations;

namespace IntegrationModul.Models
{
    public partial class Genre
    {
        [Required(ErrorMessage = "The Id for genre is required.")]
        public int Id { get; set; }
        [Required(ErrorMessage = "The name for genre is required.")]
        [StringLength (256)]
        public string Name { get; set; }
        [StringLength (1023, ErrorMessage = "The description for genre must be max length 1023 characters.")]
        public string Description { get; set; }
        public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
    }
}
