using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Printing.Models;

public partial class Organizations
{
    [Key]
    public int OrganizationID { get; set; }

    public string OrganizationName { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string? ContactPerson { get; set; }

    public string? PhoneNumber { get; set; }

    public int? SchoolID { get; set; }

    [ForeignKey("SchoolID")]
    public  School? School { get; set; }

    public  ICollection<Users> Users { get; set; } = new List<Users>();
}
