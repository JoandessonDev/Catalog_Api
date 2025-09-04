using APICatalago.Context;
using ApiCatalogo.Repositories.Interfaces;
using System.Reflection.Metadata.Ecma335;

namespace ApiCatalogo.Repositories
{
    public class UnitiOfWork : IUnitOfWork
    {
        private IProdutoRepository? _produtoRepository;
        private ICategoryRepository? _categoryRepository;
        public AppDbContext _context { get; set; }

        public UnitiOfWork(AppDbContext context)
        {
            _context = context;
        }
        

        public IProdutoRepository ProdutoRepository
        {
            get
            {
                return _produtoRepository = _produtoRepository ?? new ProdutoRepository(_context);
            }
        }

        public ICategoryRepository CategoryRepository
        {
            get
            {
                return _categoryRepository = _categoryRepository ?? new CategoriaRepository(_context);
            }
        }
        public void Commit()
        {
            _context.SaveChanges();
        }
        public void Dispose() {
            _context.Dispose();
        }


    }
}
