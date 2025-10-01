using Ecommerce.DataAccess.Services.Auth;
using Ecommerce.DataAccess.Services.Cart;
using Ecommerce.Entities.DTO.Cart;
using Ecommerce.Entities.Shared.Bases;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ResponseHandler _responseHandler;
        private readonly IValidator<UpdateCartItemRequest> _updateCartItemValidator;
        private readonly IValidator<AddingToCartRequest> _addToCartValidator;

        public CartController(ICartService cartService, ResponseHandler responseHandler, IValidator<AddingToCartRequest> addToCartValidator, IValidator<UpdateCartItemRequest> updateCartItemValidator)
        {
            _cartService = cartService;
            _responseHandler = responseHandler;
            _addToCartValidator = addToCartValidator;
            _updateCartItemValidator = updateCartItemValidator;
        }
        [HttpPost("add-to-cart")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<Response<CartResponse>>> AddToCart([FromBody] AddingToCartRequest request)
        {
            var validationResult = await _addToCartValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                string errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return StatusCode((int)_responseHandler.BadRequest<object>(errors).StatusCode,
                    _responseHandler.BadRequest<object>(errors));
            }

            var clientId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var response = await _cartService.AddingToCartAsync(clientId, request);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpGet("Cart/cart-content")]
        [Authorize(Roles = "Client")]
        public async Task<ActionResult<Response<CartResponse>>> GetClientCart()
        {
            var clientId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var response = await _cartService.GetCartAsync(clientId);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("Admin/cart-content")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<CartResponse>>> GetAdminCart(string clientId)
        {
            var response = await _cartService.GetCartAsync(clientId);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("remove/{cartItemId:guid}")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<ActionResult<Response<CartResponse>>> RemoveCartItem([FromRoute]Guid cartItemId)
        {
            var clientId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var response = await _cartService.RemoveCartItemAsync(clientId, cartItemId);
            return StatusCode((int)response.StatusCode, response);
        }


        [HttpDelete("clear-cart")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<ActionResult<Response<CartResponse>>>ClearCart()
        {
            var clientId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var response = await _cartService.ClearCartItemsAsync(clientId);
            return StatusCode((int)response.StatusCode, response);
        }
    }


}
