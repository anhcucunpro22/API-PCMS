using Printing.Models;
using System.ComponentModel.DataAnnotations;

namespace Printing.AppModels
{
    public class ReceiptCreationDto
    {
        public ReceiptDetail ReceiptDetail { get; set; }
        public List<int> ServiceIds { get; set; }
    }
}
