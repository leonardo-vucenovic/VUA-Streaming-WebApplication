using System.ComponentModel.DataAnnotations;

namespace IntegrationModul.Models
{
    public class User
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DeletedAt { get; set; }


        [Required(ErrorMessage = "The username is required")]
        [StringLength(256)]
        public string Username { get; set; }


        [Required(ErrorMessage = "The firstname is required")]
        [StringLength(256)]
        public string FirstName { get; set; }


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

        public int CountryOfResidenceId { get; set; }
        public virtual Country CountryOfResidence { get; set; } = null!;

    }
}
