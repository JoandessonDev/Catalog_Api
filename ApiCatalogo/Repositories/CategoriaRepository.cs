using APICatalago.Context;
using APICatalago.Models;
using ApiCatalogo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Repositories
{
    public class CategoriaRepository : Repository<Categoria>, ICategoryRepository
    {

        public CategoriaRepository(AppDbContext context) : base(context)
        {
        }

    }
}
