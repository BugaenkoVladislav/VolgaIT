using System;
using System.Collections.Generic;

namespace api.Database.Models;

public partial class Color
{
    public long Id { get; set; }

    public string Color1 { get; set; } = null!;

    public virtual ICollection<Transport> Transports { get; set; } = new List<Transport>();
}
