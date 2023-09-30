using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.API.DTOs.Product;
using Product.API.Entities;
using Product.API.Repositories.Interface;
using System.ComponentModel.DataAnnotations;

namespace Product.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        private static int count = 0;

        public ProductsController(
            IProductRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _repository.GetProducts();
            var result = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetProduct([Required] long id)
        {
            var product = await _repository.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<ProductDto>(product);
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        {
            var existingProduct = await _repository.GetProductByNo(productDto.No);
            if (existingProduct != null)
            {
                return BadRequest($"Product No: {existingProduct.No} is existed");
            }

            var product = _mapper.Map<CatalogProduct>(productDto);
            await _repository.CreateAsync(product);
            await _repository.SaveChangesAsync();
            var result = _mapper.Map<ProductDto>(product);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateProduct([Required] long id, [FromBody] UpdateProductDto productDto)
        {
            var product = await _repository.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }

            var updateProduct = _mapper.Map(productDto, product);
            await _repository.UpdateProduct(updateProduct);
            await _repository.SaveChangesAsync();

            var result = _mapper.Map<ProductDto>(updateProduct);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteProduct([Required] long id)
        {
            var product = await _repository.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }

            await _repository.DeleteProduct(id);
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("get-product-by-no/{productNo}")]
        public async Task<IActionResult> GetProductByNo([Required] string productNo)
        {
            var product = await _repository.GetProductByNo(productNo);
            if (product == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<ProductDto>(product);
            return Ok(result);
        }
    }
}
