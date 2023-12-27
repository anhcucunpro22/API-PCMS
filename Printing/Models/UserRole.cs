using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Printing.Models;

public  class UserRole
{
    [Key]
    public int UserRoleID { get; set; }
    public int UserID { get; set; }

    public int RoleID { get; set; }

    [ForeignKey("UserID")]

    public Users User { get; set; } = null!;

    [ForeignKey("RoleID")]
    public  Role Role { get; set; } = null!;

   
    
}
