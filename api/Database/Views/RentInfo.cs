using System;
using System.Collections.Generic;

namespace api.Database.Views;

public partial class RentInfo
{
    public long? Id { get; set; }

    public long? UserId { get; set; }

    public long? TransportId { get; set; }

    public long? OwnerId { get; set; }

    public DateTime? TimeStart { get; set; }

    public DateTime? TimeEnd { get; set; }

    public string? PriceType { get; set; }

    public double? PriceOfUnit { get; set; }

    public double? FinalPrice { get; set; }
}
