using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PublicModul.ViewModels
{
    public class VMUser
    {
        public int Id { get; set; }
        [DisplayName("Created")]
        public DateTime CreatedAt { get; set; }
        [DisplayName("Deleted")]
        public DateTime DeletedAt { get; set; }

        [DisplayName("User name")]
        [Required(ErrorMessage = "The username is required")]
        [StringLength(256)]
        public string Username { get; set; }


        [DisplayName("First name")]
        [Required(ErrorMessage = "The firstname is required")]
        [StringLength(256)]
        public string FirstName { get; set; }


        [DisplayName("Last name")]
        [Required(ErrorMessage = "The username is required")]
        [StringLength(256)]
        public string LastName { get; set; }


        [Required(ErrorMessage = "The email is required")]
        [StringLength(256)]
        public string Email { get; set; }

        public string PwdHash { get; set; } = null!;

        public string PwdSalt { get; set; } = null!;

        public string Phone { get; set; }

        public bool IsConfirmed { get; set; }

        public string SecurityToken { get; set; }
        [DisplayName("Counutry ID")]
        public int CountryOfResidenceId { get; set; }
        [DisplayName("Counutry")]
        public virtual VMCountry CountryOfResidence { get; set; } = null!;
    }
}
