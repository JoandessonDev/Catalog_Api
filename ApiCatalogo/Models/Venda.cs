
using ApiCatalogo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICatalago.Models
{
    public class Venda
    {
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public DateTime DataVenda { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [StringLength(20)]
        public string? Status { get; set; } = "Pago";

        public ICollection<VendaItem>? Itens { get; set; }
    }
}
