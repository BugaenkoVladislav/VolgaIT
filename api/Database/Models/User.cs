using System;
using System.Collections.Generic;

namespace api.Database.Models;

public partial class User
{
    public long Id { get; set; }

    public string Password { get; set; } = null!;

    public double Balance { get; set; }

    public string Username { get; set; } = null!;

    public bool IsAdmin { get; set; }

    public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();

    public virtual ICollection<Transport> Transports { get; set; } = new List<Transport>();
}
