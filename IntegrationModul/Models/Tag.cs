using BusinessLayer.DALModels;
using System.ComponentModel.DataAnnotations;

namespace IntegrationModul.Models
{
    public partial class Tag
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The name for tag is required")]
        [StringLength(256)]
        public string Name { get; set; }
        public virtual ICollection<VideoTag> VideoTags { get; set; } = new List<VideoTag>();
    }
}
