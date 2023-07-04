using BusinessLayer.DALModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.BLModels
{
    public class BLUser
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? DeletedAt { get; set; } = null!;

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PwdHash { get; set; }

        public string PwdSalt { get; set; }

        public string Phone { get; set; }

        public bool IsConfirmed { get; set; }

        public string SecurityToken { get; set; }

        public int CountryOfResidenceId { get; set; }

        public virtual BLCountry CountryOfResidence { get; set; } = null!;
    }
}
