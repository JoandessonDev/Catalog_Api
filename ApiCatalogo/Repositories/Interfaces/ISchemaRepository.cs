using ApiCatalogo.Models.DTOs;

namespace ApiCatalogo.Repositories.Interfaces
{
    public interface ISchemaRepository
    {
        Task<List<EntitySchemaDTO>> GetSchemaAsync();
    }
}
