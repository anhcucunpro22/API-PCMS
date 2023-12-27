using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Printing.Models;

public partial class Suppliers
{
    [Key]
    public int SupplierID { get; set; }

    public string SupplierName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string ContactName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public  ICollection<InventoryIn> InventoryIns { get; set; } = new List<InventoryIn>();
}
