using System.ComponentModel.DataAnnotations;

namespace PublicModul.ViewModels
{
    public class VMCountry
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The code for country is required.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "The code for country must be exactly 2 characters.")]
        public string Code { get; set; }
        [Required(ErrorMessage = "The name for country if required.")]
        [StringLength(256, MinimumLength = 1, ErrorMessage = "The name for country must be max length 256 characters.")]
        public string Name { get; set; }
        public virtual ICollection<VMUser> Users { get; set; } = new List<VMUser>();
    }
}
