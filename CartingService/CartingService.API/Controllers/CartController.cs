using AutoMapper;
using CartingService.API.Models;
using CartingService.BLL;
using CartingService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace CartingService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class CartController : ControllerBase
    {
        private readonly ICartService cartService;
        private readonly IMapper mapper;

        public CartController(ICartService cartService, IMapper mapper)
        {
            this.cartService = cartService;
            this.mapper = mapper;
        }

        /// <summary>
        ///  Get cart info.
        /// </summary>
        /// <param name="key">cart unique Key</param>
        /// <returns>a cart model(cart key + list of cart items)</returns>
        [MapToApiVersion("1")]
        [HttpGet("{key}", Name = "GetByCartId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<CartModel> Get(Guid key)
        {
            var items = cartService.GetCartItems(key);

            return Ok(new CartModel { CartKey = key, Items = items.Select(x => mapper.Map<ItemModel>(x)).ToList() });
        }

        /// <summary>
        /// Add item to cart
        /// </summary>
        /// <param name="cartKey">cart unique Key (string)</param>
        /// <param name="itemModel">cart item model</param>
        /// <returns>Returns 200 if item was added to the cart. If there was no cart for specified key – creates it. Otherwise returns a corresponding HTTP code.</returns>
       
        [HttpPost("{cartKey}", Name = "AddItem")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Add(Guid cartKey, ItemModel itemModel)
        {
            var modelForAdding = mapper.Map<CartItem>(itemModel);

            try
            {
                this.cartService.AddItemToCart(cartKey, modelForAdding);
                return new OkResult();
            }
            catch
            {
                return new BadRequestResult();
            }
        }


        /// <summary>
        /// Delete item from cart.
        /// </summary>
        /// <param name="cartKey">cart unique key (string)</param>
        /// <param name="itemId">item id (int)</param>
        /// <returns>Returns 200 if item was deleted, otherwise returns corresponding HTTP code</returns>
       
        [HttpDelete("{cartKey}/items/{itemId}", Name = "DeleteItem")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult Delete(Guid cartKey, Guid itemId)
        {
            this.cartService.RemoveItemFromCart(cartKey, itemId);
            return new OkResult();
        }
    }
}
