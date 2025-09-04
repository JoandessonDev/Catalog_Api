
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APICatalago.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Telefone { get; set; }

        [StringLength(250)]
        public string? Endereco { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        public ICollection<Venda>? Vendas { get; set; }
    }
}
