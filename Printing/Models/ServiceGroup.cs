using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Printing.Models;

public partial class ServiceGroup
{
    [Key]
    public int GroupID { get; set; }

    public string GroupName { get; set; } = null!;

    public  ICollection<Services> Services { get; set; } = new List<Services>();
}
