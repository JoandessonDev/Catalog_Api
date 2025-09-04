using APICatalago.Context;
using APICatalago.Models;
using ApiCatalogo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace ApiCatalogo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductController> _logger;
        private readonly ISchemaRepository _schemaRepository;

        public ProductController(IUnitOfWork unitOfWork, ILogger<ProductController> logger, ISchemaRepository schemaRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _schemaRepository = schemaRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            var schemaInfo = _schemaRepository.GetSchemaAsync();
            var products = _unitOfWork.ProdutoRepository.GetAll();
            if (products is null || !products.Any())
            {
                _logger.LogWarning("No products found");
                return NotFound("No products found");
            }

            return Ok(products);
        }

        [HttpGet("{id:int}", Name = "GetSingleProduct")]
        public ActionResult<Produto> GetProduct(int id)
        {
            var product = _unitOfWork.ProdutoRepository.Get(p => p.Id == id);
            if (product is null)
            {
                _logger.LogWarning("Product with id {ProductId} not found", id);
                return NotFound($"Product with id = {id} not found");
            }

            return Ok(product);
        }

        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            if (produto is null)
            {
                _logger.LogWarning("Attempted to create null product");
                return BadRequest("Invalid product");
            }

            var newProduct = _unitOfWork.ProdutoRepository.Create(produto);
            _unitOfWork.Commit();
            _logger.LogInformation("Product created with id {ProductId}", newProduct.Id);

            return new CreatedAtRouteResult("GetSingleProduct", new { id = newProduct.Id }, newProduct);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            if (produto is null)
            {
                return BadRequest("Invalid product");
            }

            if (id != produto.Id)
            {
                return BadRequest("Id mismatch");
            }

            var updatedProduct = _unitOfWork.ProdutoRepository.Update(produto);
            _unitOfWork.Commit();

            if (updatedProduct is null)
            {
                return StatusCode(500, $"Failed to update product with id = {id}");
            }

            _logger.LogInformation("Product with id {ProductId} updated", id);
            return Ok(produto);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var product = _unitOfWork.ProdutoRepository.Get(p => p.Id == id);
            if (product is null)
            {
                _logger.LogWarning("Product with id {ProductId} not found for deletion", id);
                return NotFound($"Product with id = {id} not found");
            }

            var deleted = _unitOfWork.ProdutoRepository.Delete(product);
            _unitOfWork.Commit();

            if (deleted is null)
                return StatusCode(500, $"Failed to delete product id = {id}");

            _logger.LogInformation("Product with id {ProductId} deleted", id);
            return Ok("Product deleted successfully");
        }

        [HttpGet("products/category/{categoryId}")]
        public ActionResult<IEnumerable<Produto>> GetProductsByCategory(int categoryId)
        {
            var products = _unitOfWork.ProdutoRepository.GetProdutosPorCategoria(categoryId);
            if (products is null || !products.Any())
            {
                _logger.LogWarning("No products found for category id {CategoryId}", categoryId);
                return NotFound($"No products found for category id = {categoryId}");
            }

            return Ok(products);
        }
    }
}
