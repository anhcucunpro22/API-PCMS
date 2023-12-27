using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Printing.Models;

public class Warehouses
{
    [Key]
    public int WarehouseID { get; set; }

    public string WarehouseName { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string ManagerNameWh { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public  ICollection<InventoryIn> InventoryIns { get; set; } = new List<InventoryIn>();

    public  ICollection<InventoryOut> InventoryOuts { get; set; } = new List<InventoryOut>();
}
