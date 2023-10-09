using System;
using System.Collections.Generic;

namespace api.Database.Models;

public partial class RentType
{
    public long Id { get; set; }

    public string RentType1 { get; set; } = null!;

    public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();
}
