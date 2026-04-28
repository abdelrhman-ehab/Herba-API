using HerbCare.Data;
using HerbCare.DTOs;
using HerbCare.DTOs.OrderDTOs;
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
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<ApiResponseDTO<OrderDTO>>> Checkout(CreateOrderDTO model)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new ApiResponseDTO<OrderDTO>
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.CartId == model.CartId && c.UserId == userId);

                if (cart == null || !cart.CartItems.Any())
                {
                    return BadRequest(new ApiResponseDTO<OrderDTO>
                    {
                        Success = false,
                        Message = "Cart is empty or not found"
                    });
                }

                var totalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price);

                var order = new OrderPayment
                {
                    OrderId = new Random().Next(1000, 9999),
                    UserId = userId,
                    CartId = cart.CartId,
                    Location = model.Location,
                    Quantity = cart.Quantity,
                    TotalAmount = totalAmount,
                    Date = DateTime.UtcNow,
                    PaymentMethod = model.PaymentMethod
                };

                _context.OrderPayments.Add(order);

                // Clear the cart after checkout
                _context.CartItems.RemoveRange(cart.CartItems);
                cart.Quantity = 0;

                await _context.SaveChangesAsync();

                var orderDTO = new OrderDTO
                {
                    PaymentId = order.PaymentId,
                    OrderId = order.OrderId,
                    Location = order.Location,
                    Quantity = order.Quantity,
                    TotalAmount = order.TotalAmount,
                    Date = order.Date,
                    PaymentMethod = order.PaymentMethod,
                    Items = cart.CartItems.Select(ci => new OrderItemDTO
                    {
                        ProductName = ci.Product.Name,
                        Quantity = ci.Quantity,
                        Price = ci.Product.Price
                    }).ToList()
                };

                return Ok(new ApiResponseDTO<OrderDTO>
                {
                    Success = true,
                    Message = "Order placed successfully",
                    Data = orderDTO
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<OrderDTO>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<OrderDTO>>>> GetMyOrders()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new ApiResponseDTO<IEnumerable<OrderDTO>>
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                var orders = await _context.OrderPayments
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.Date)
                    .Select(o => new OrderDTO
                    {
                        PaymentId = o.PaymentId,
                        OrderId = o.OrderId,
                        Location = o.Location,
                        Quantity = o.Quantity,
                        TotalAmount = o.TotalAmount,
                        Date = o.Date,
                        PaymentMethod = o.PaymentMethod
                    })
                    .ToListAsync();

                return Ok(new ApiResponseDTO<IEnumerable<OrderDTO>>
                {
                    Success = true,
                    Data = orders
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<IEnumerable<OrderDTO>>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}