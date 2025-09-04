namespace ApiCatalogo.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IProdutoRepository ProdutoRepository { get; }
        ICategoryRepository CategoryRepository { get; }

        void Commit();
    }
}
