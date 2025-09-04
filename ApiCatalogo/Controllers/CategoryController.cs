using APICatalago.Context;
using APICatalago.Models;
using ApiCatalogo.Filters;
using ApiCatalogo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace ApiCatalogo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(IUnitOfWork unitOfWork, ILogger<CategoryController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiloggingFilter))]
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            var categories = _unitOfWork.CategoryRepository.GetAll();
            if (categories is null || !categories.Any())
            {
                _logger.LogWarning("No categories found");
                return NotFound("No categories found");
            }

            return Ok(categories);
        }

        [HttpGet("{id:int}", Name = "GetCategoryById")]
        public ActionResult<Categoria> GetById(int id)
        {
            var category = _unitOfWork.CategoryRepository.Get(c => c.Id == id);
            if (category is null)
            {
                _logger.LogWarning("Category with id {CategoryId} not found", id);
                return NotFound($"Category with id = {id} not found");
            }

            return Ok(category);
        }

        [HttpPost]
        public ActionResult Post(Categoria categoria)
        {
            if (categoria is null)
            {
                _logger.LogWarning("Attempted to create null category");
                return BadRequest("Invalid category");
            }

            var newCategory = _unitOfWork.CategoryRepository.Create(categoria);
            _unitOfWork.Commit();
            _logger.LogInformation("Category created with id {CategoryId}", newCategory.Id);

            return new CreatedAtRouteResult("GetCategoryById", new { id = newCategory.Id }, newCategory);
        }

        [HttpPut("{id:int}")]
        public ActionResult<Categoria> Update(int id, Categoria categoria)
        {
            if (categoria is null)
            {
                return BadRequest("Invalid category");
            }

            if (id != categoria.Id)
            {
                return BadRequest("Id mismatch");
            }

            var updated = _unitOfWork.CategoryRepository.Update(categoria);
            _unitOfWork.Commit();

            if (updated is null)
                return StatusCode(500, $"Failed to update category id = {id}");

            _logger.LogInformation("Category with id {CategoryId} updated", id);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var category = _unitOfWork.CategoryRepository.Get(c => c.Id == id);
            if (category is null)
            {
                _logger.LogWarning("Category with id {CategoryId} not found for deletion", id);
                return NotFound($"Category with id = {id} not found");
            }

            var deleted = _unitOfWork.CategoryRepository.Delete(category);
            _unitOfWork.Commit();

            if (deleted is null)
                return StatusCode(500, $"Failed to delete category id = {id}");

            _logger.LogInformation("Category with id {CategoryId} deleted", id);
            return Ok("Category deleted successfully");
        }
    }
}
