using System.ComponentModel.DataAnnotations;

namespace PublicModul.ViewModels
{
    public class VMImage
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The content for image is required.")]
        public virtual ICollection<VMVideo> Videos { get; set; } = new List<VMVideo>();
    }
}
