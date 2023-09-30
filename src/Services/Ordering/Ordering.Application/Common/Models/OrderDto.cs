using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Ordering.Application.Common.Mappings;
using Ordering.Domain.Entities;
using Ordering.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ordering.Application.Common.Models
{
    public class OrderDto : IMapFrom<Order>
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public decimal TotalPrice { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string ShippingAddress { get; set; }
        public string InvoiceAddress { get; set; }
        public string Status { get; set; }
    }
}
