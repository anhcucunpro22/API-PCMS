using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Printing.Models;

public partial class Material
{
    [Key]
    public int MaterialID { get; set; }

    public int? GroupID { get; set; }

    public string MaterialName { get; set; } = null!;

    public decimal? Price { get; set; }

    [ForeignKey("GroupID")]
    public  MaterialGroup? Group { get; set; }

    public  ICollection<InventoryInDetail> InventoryInDetails { get; set; } = new List<InventoryInDetail>();

    public  ICollection<InventoryOutDetail> InventoryOutDetails { get; set; } = new List<InventoryOutDetail>();
}
