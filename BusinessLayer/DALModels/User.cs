using System;
using System.Collections.Generic;

namespace BusinessLayer.DALModels;

public partial class User
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string Username { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string PwdHash { get; set; }

    public string PwdSalt { get; set; }

    public string Phone { get; set; }

    public bool IsConfirmed { get; set; }

    public string SecurityToken { get; set; } = null!;

    public int CountryOfResidenceId { get; set; }

    public virtual Country CountryOfResidence { get; set; } = null!;
}
