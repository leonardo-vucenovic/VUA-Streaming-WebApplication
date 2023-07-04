using System.ComponentModel.DataAnnotations;

namespace AdministrativeModul.ViewModels
{
    public class VMImage
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The content for image is required.")]
        public string Content { get; set; }
        public virtual ICollection<VMVideo> Videos { get; set; } = new List<VMVideo>();
    }
}
