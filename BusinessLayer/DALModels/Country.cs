using System;
using System.Collections.Generic;

namespace BusinessLayer.DALModels;

public partial class Country
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
