using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Printing.Models;

public  class UserFacilities
{
    [Key]
    public int UserFaID { get; set; }
    public int UserID { get; set; }

    public int FacilityID { get; set; }

    [ForeignKey("UserID")]
    public Users User { get; set; } = null!;

    [ForeignKey("FacilityID")]
    public Facilities Facility { get; set; } = null!;

    
}
