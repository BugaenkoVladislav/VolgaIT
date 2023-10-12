using System;
using System.Collections.Generic;

namespace api.Database.Views;

public partial class RentInfo
{
    public long? Id { get; set; }

    public string? User { get; set; }

    public long? IdTransport { get; set; }

    public string? Owner { get; set; }
}
