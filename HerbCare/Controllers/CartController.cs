using HerbCare.Data;
using HerbCare.DTOs;
using HerbCare.DTOs.StoreDTOs;
using HerbCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HerbCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO<CartDTO>>> GetCart()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new ApiResponseDTO<CartDTO>
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId,
                        Quantity = 0,
                        CreatedDate = DateTime.UtcNow
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                var cartDTO = new CartDTO
                {
                    CartId = cart.CartId,
                    Items = cart.CartItems.Select(ci => new CartItemDTO
                    {
                        ProductId = ci.ProductId,
                        ProductName = ci.Product.Name,
                        Quantity = ci.Quantity,
                        UnitPrice = ci.Product.Price,
                        TotalPrice = ci.Quantity * ci.Product.Price
                    }).ToList(),
                    TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price)
                };

                return Ok(new ApiResponseDTO<CartDTO>
                {
                    Success = true,
                    Data = cartDTO
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<CartDTO>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult<ApiResponseDTO<CartDTO>>> AddToCart(AddToCartDTO model)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new ApiResponseDTO<CartDTO>
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId,
                        Quantity = 0,
                        CreatedDate = DateTime.UtcNow
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                var product = await _context.Products.FindAsync(model.ProductId);
                if (product == null)
                {
                    return NotFound(new ApiResponseDTO<CartDTO>
                    {
                        Success = false,
                        Message = "Product not found"
                    });
                }

                var cartItem = cart.CartItems
                    .FirstOrDefault(ci => ci.ProductId == model.ProductId);

                if (cartItem == null)
                {
                    cartItem = new CartItem
                    {
                        CartId = cart.CartId,
                        ProductId = model.ProductId,
                        Quantity = model.Quantity
                    };
                    _context.CartItems.Add(cartItem);
                }
                else
                {
                    cartItem.Quantity += model.Quantity;
                }

                cart.Quantity += model.Quantity;
                await _context.SaveChangesAsync();

                return await GetCart();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<CartDTO>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("remove/{productId}")]
        public async Task<ActionResult<ApiResponseDTO<CartDTO>>> RemoveFromCart(int productId)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new ApiResponseDTO<CartDTO>
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    return NotFound(new ApiResponseDTO<CartDTO>
                    {
                        Success = false,
                        Message = "Cart not found"
                    });
                }

                var cartItem = cart.CartItems
                    .FirstOrDefault(ci => ci.ProductId == productId);

                if (cartItem == null)
                {
                    return NotFound(new ApiResponseDTO<CartDTO>
                    {
                        Success = false,
                        Message = "Item not found in cart"
                    });
                }

                cart.Quantity -= cartItem.Quantity;
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                return await GetCart();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<CartDTO>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}