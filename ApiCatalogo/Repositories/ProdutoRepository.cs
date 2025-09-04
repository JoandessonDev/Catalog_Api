using APICatalago.Context;
using APICatalago.Models;
using ApiCatalogo.Controllers;
using ApiCatalogo.Repositories.Interfaces;

namespace ApiCatalogo.Repositories
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext context) : base(context)
        {
        }

        public IEnumerable<Produto> GetProdutosPorCategoria(int id)
        {
            return GetAll().Where(p => p.CategoriaId == id);
        }

    }
}
