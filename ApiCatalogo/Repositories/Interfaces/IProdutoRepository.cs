using APICatalago.Models;

namespace ApiCatalogo.Repositories.Interfaces
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        IEnumerable<Produto> GetProdutosPorCategoria(int id);
    }
}
