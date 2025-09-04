
using APICatalago.Context;
using ApiCatalogo.Models.DTOs;
using ApiCatalogo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ApiCatalogo.Repositories
{
    public class SchemaRepository : ISchemaRepository
    {
        private readonly AppDbContext _context;

        public SchemaRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<EntitySchemaDTO>> GetSchemaAsync()
        {
            var model = _context.Model.GetEntityTypes()
                .Select(et => new EntitySchemaDTO
                {
                    TableName = et.GetTableName(),
                    ClrType = et.ClrType.FullName,
                    EntityName = et.ClrType.Name,
                    Properties = et.GetProperties()
                        .Select(p => new EntityPropertyDTO
                        {
                            Name = p.Name,
                            ColumnName = p.GetColumnName(
                                StoreObjectIdentifier.Table(et.GetTableName(), et.GetSchema())),
                            Type = p.ClrType.Name,
                            IsPrimaryKey = p.IsPrimaryKey()
                        }).ToList()
                })
                .ToList();

            if (model == null || !model.Any())
                throw new Exception("Schema not found");

            return Task.FromResult(model);
        }

    }
}
