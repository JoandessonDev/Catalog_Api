using ApiCatalogo.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICatalago.Models
{
    public class Produto
    {
        public int  Id { get; set; }
        [Required(ErrorMessage = "Nome é obrigatório.")]
        [StringLength(20, ErrorMessage = "Nome supera o tamanho máximo")]
        [FirstLetterUpper]
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public string Preco { get; set; }
        public string? ImageUrl { get; set; }
        public float Estoque { get; set; }
        public DateTime DataCadastro { get; set; } 
        public int CategoriaId { get; set; }
        [NotMapped]
        public Categoria? Categoria { get; set; }
    }
}
