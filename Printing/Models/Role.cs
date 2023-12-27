using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Printing.Models;

public partial class Role
{
    [Key]
    public int RoleID { get; set; }

    public string RoleName { get; set; } = null!;

    public  ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
