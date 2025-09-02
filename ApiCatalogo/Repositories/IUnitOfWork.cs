namespace ApiCatalogo.Repositories
{
    public interface IUnitOfWork
    {
        IProdutoRepository ProdutoRepository { get; }
        ICategoryRepository CategoryRepository { get; }

        void Commit();
    }
}
