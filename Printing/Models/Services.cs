using Printing.AppModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Printing.Models;

public class Services
{
    [Key]
    public int ServiceID { get; set; }

    public int? GroupID { get; set; }

    public string? ServiceName { get; set; }

    public string? Dvt { get; set; }

    public decimal? Price { get; set; }

    public string? Description { get; set; }

    public int? FacilityID { get; set; }

    public void ServiceUpdateDto(ServiceUpdateDto ServiceUpdateDto)
    {
        Description = ServiceUpdateDto.Description;
    }

    [ForeignKey("FacilityID")]
    public Facilities? Facility { get; set; }

    [ForeignKey("GroupID")]
    public ServiceGroup? Group { get; set; }

    public ICollection<ReceiptDetail>? ReceiptDetails { get; set; }
}