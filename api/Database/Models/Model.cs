using System;
using System.Collections.Generic;

namespace api.Database.Models;

public partial class Model
{
    public long Id { get; set; }

    public string Model1 { get; set; } = null!;

    public virtual ICollection<Transport> Transports { get; set; } = new List<Transport>();
}
