using HerbCare.Data;
using HerbCare.DTOs;
using HerbCare.DTOs.StoreDTOs;
using HerbCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HerbCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StoreController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<StoreDTO>>>> GetStores()
        {
            try
            {
                var stores = await _context.Stores
                    .Include(s => s.StoreLocations)
                    .Select(s => new StoreDTO
                    {
                        StoreId = s.StoreId,
                        Name = s.Name,
                        ContactInfo = s.ContactInfo,
                        Locations = s.StoreLocations.Select(sl => sl.Location).ToList()
                    })
                    .ToListAsync();

                return Ok(new ApiResponseDTO<IEnumerable<StoreDTO>>
                {
                    Success = true,
                    Data = stores
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<IEnumerable<StoreDTO>>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("{id}/products")]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<ProductDTO>>>> GetStoreProducts(int id)
        {
            try
            {
                var products = await _context.StoreProducts
                    .Where(sp => sp.StoreId == id)
                    .Include(sp => sp.Product)
                    .Select(sp => new ProductDTO
                    {
                        ProductId = sp.Product.ProductId,
                        Name = sp.Product.Name,
                        Image = sp.Product.Image,
                        Price = sp.Product.Price
                    })
                    .ToListAsync();

                return Ok(new ApiResponseDTO<IEnumerable<ProductDTO>>
                {
                    Success = true,
                    Data = products
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<IEnumerable<ProductDTO>>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("products")]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<ProductDTO>>>> GetAllProducts()
        {
            try
            {
                var products = await _context.Products
                    .Select(p => new ProductDTO
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Image = p.Image,
                        Price = p.Price
                    })
                    .ToListAsync();

                return Ok(new ApiResponseDTO<IEnumerable<ProductDTO>>
                {
                    Success = true,
                    Data = products
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<IEnumerable<ProductDTO>>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}