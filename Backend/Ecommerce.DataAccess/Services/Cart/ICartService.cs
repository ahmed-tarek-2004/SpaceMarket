using Ecommerce.Entities.DTO.Cart;
using Ecommerce.Entities.Shared.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Services.Cart
{
   public interface ICartService
    {
        Task<Response<CartResponse>> AddingToCartAsync(string clientId, AddingToCartRequest request);
        Task<Response<CartResponse>> GetCartAsync(string clientId);
        Task<Response<CartResponse>> UpdateCartItemQuantityAsync(string clientId, UpdateCartItemRequest request);
        Task<Response<CartResponse>> RemoveCartItemAsync(string clientId, Guid cartItemId);
        public Task<Response<CartResponse>> ClearCartItemsAsync(string clientId);
    }
}
