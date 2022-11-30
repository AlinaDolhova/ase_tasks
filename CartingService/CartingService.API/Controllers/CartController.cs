using AutoMapper;
using CartingService.API.Models;
using CartingService.BLL;
using CartingService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<CartController> logger;

        public CartController(ICartService cartService, IMapper mapper, ILogger<CartController> logger)
        {
            this.cartService = cartService;
            this.mapper = mapper;
            this.logger = logger;
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
            logger.LogInformation("Carting service GET for cart id {key} received", key);

            var items = cartService.GetCartItems(key).ToList();

            logger.LogInformation("Carting service GET for cart id {key} completed: {items.Count} found", key, items.Count);
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
            logger.LogInformation("Carting service POST for cart id {cartKey} received", cartKey);

            var modelForAdding = mapper.Map<CartItem>(itemModel);

            try
            {
                this.cartService.AddItemToCart(cartKey, modelForAdding);
                logger.LogInformation("Carting service POST for cart id {cartKey} completed", cartKey);

                return new OkResult();
            }
            catch (Exception e)
            {
                logger.LogError("Carting service POST for cart id {cartKey} failed", cartKey, e);
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
            logger.LogInformation("Carting service DELETE for cart id {cartKey}, item id {itemId} received", cartKey, itemId);

            this.cartService.RemoveItemFromCart(cartKey, itemId);

            logger.LogInformation("Carting service DELETE for cart id {cartKey}, item id {itemId} completed", cartKey, itemId);
            return new OkResult();
        }
    }
}
