// Models/VendaItem.cs
using ApiCatalogo.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICatalago.Models
{
    public class VendaItem
    {
        public int Id { get; set; }

        [Required]
        public int VendaId { get; set; }
        public Venda? Venda { get; set; }

        [Required]
        public int ProdutoId { get; set; }
        public Produto? Produto { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoUnitario { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal Quantidade { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; } 
    }
}
