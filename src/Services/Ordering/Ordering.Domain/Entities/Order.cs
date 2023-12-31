﻿using Contracts.Common.Events;
using Ordering.Domain.OrderAggregate.Events;
using Shared.Enums.Order;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordering.Domain.Entities
{
    public class Order : AuditableEventEntity<long>
    {
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string UserName { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string DocumentNo { get; set; }


        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string FirstName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Column(TypeName = "nvarchar(250)")]
        public string EmailAddress { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string ShippingAddress { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string InvoiceAddress { get; set; }
        public OrderStatus Status { get; set; }

        public Order AddedOrder()
        {
            AddDomainEvent(new OrderCreatedEvent(
                Id, UserName, TotalPrice, FirstName, LastName,
                EmailAddress, ShippingAddress, InvoiceAddress,
                Status));
            return this;
        }

        public Order DeletedOrder()
        {
            AddDomainEvent(new OrderDeletedEvent(Id));
            return this;
        }

    }
}
