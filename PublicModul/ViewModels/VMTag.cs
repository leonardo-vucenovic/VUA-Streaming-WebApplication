using System.ComponentModel.DataAnnotations;

namespace PublicModul.ViewModels
{
    public class VMTag
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The name for tag is required")]
        [StringLength(256)]
        public string Name { get; set; }
        public virtual ICollection<VMVideoTag> VideoTags { get; set; } = new List<VMVideoTag>();
    }
}
