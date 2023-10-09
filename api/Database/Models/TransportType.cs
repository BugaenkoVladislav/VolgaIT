using System;
using System.Collections.Generic;

namespace api.Database.Models;

public partial class TransportType
{
    public long Id { get; set; }

    public string TransportType1 { get; set; } = null!;

    public virtual ICollection<Transport> Transports { get; set; } = new List<Transport>();
}
