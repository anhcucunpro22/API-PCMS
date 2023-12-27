using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Printing.Models;

public partial class UnitOfMeasure
{
    [Key]
    public int UnitID { get; set; }

    public string UnitName { get; set; } = null!;

    public string? ConversionFactor { get; set; }

    public string? Description { get; set; }

    public ICollection<InventoryInDetail> InventoryInDetails { get; set; } = new List<InventoryInDetail>();

    public ICollection<InventoryOutDetail> InventoryOutDetails { get; set; } = new List<InventoryOutDetail>();
}
