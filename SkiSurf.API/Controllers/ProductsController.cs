using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiSurf.Infrastructure.Data;
using SkiSurf.Core.Entities;
using SkiSurf.Core.Interfaces;
using SkiSurf.Core.Specifications;
using SkiSurf.API.DTOs;
using AutoMapper;
using SkiSurf.API.Errors;
using SkiSurf.API.Helpers;

namespace SkiSurf.API.Controllers
{
    
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandRepo;
        private readonly ILogger<ProductsController> _logger;

        public IGenericRepository<ProductType> _productTypeRepo { get; }

        private IMapper _mapper;

        public ProductsController(ILogger<ProductsController> logger, IGenericRepository<Product> productsRepo,IGenericRepository<ProductBrand> productBrandRepo, IGenericRepository<ProductType> productTypeRepo, IMapper mapper) {
            _productsRepo = productsRepo;
            _productBrandRepo = productBrandRepo;
            _productTypeRepo = productTypeRepo;
            _mapper=mapper;
            _logger = logger;
        }


        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams Productparams) {
        
            var spec = new ProductsWithTypesAndBrandsSpecification(Productparams);
            var countSpec = new ProductWithFiltersForCountSpecification(Productparams);
            var totalItems = await _productsRepo.CountAsync(countSpec);
            var products = await _productsRepo.ListAsync(spec);
         
            var data=_mapper.Map<IReadOnlyList<Product>,IReadOnlyList<ProductToReturnDto>>(products);
            //return products.Select(product=>new ProductToReturnDto
            //{
            //    Id = product.Id,
            //    Name = product.Name,
            //    Description = product.Description,
            //    PictureUrl = product.PictureUrl,
            //    Price = product.Price,
            //    ProductBrand = product.ProductBrand.Name,
            //    ProductType = product.ProductType.Name
            //}).ToList();
            return Ok(new Pagination<ProductToReturnDto>(Productparams.PageIndex,Productparams.PageSize,totalItems,data));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);
            var product = await _productsRepo.GetEntityWithSpec(spec);

          
            try
            {
                if (product == null)
                {
                    return NotFound(new ApiResponse(404));
                }
                return _mapper.Map<Product, ProductToReturnDto>(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error occurred while fetching product by id.");
                return StatusCode(500, "An error occurred while processing your request.");

            }

        }

        [HttpGet("brands")]
        public async Task<ActionResult<Product>> GetProductBrands()
        {
            try {
                return Ok(await _productBrandRepo.ListAllAsync());
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message, "Error occurred while fetching product brands.");
                return StatusCode(500, "An error occurred while processing your request.");

            }
          


        }

        [HttpGet("types")]
        public async Task<ActionResult<Product>> GetProductTypes()
        {
            return Ok(await _productTypeRepo.ListAllAsync());
        }





    }
}
