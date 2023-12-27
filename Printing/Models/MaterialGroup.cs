using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Printing.Models;

public partial class MaterialGroup
{
    [Key]
    public int GroupID { get; set; }

    public string GroupName { get; set; } = null!;

    public ICollection<Material> Materials { get; set; } = new List<Material>();
}
