using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Printing.Models;

public partial class School
{
    [Key]
    public int SchoolID { get; set; }

    public string SchoolName { get; set; } = null!;

    public  ICollection<Organizations> Organizations { get; set; } = new List<Organizations>();
}
