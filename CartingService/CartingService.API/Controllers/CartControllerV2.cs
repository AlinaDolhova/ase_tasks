using CartingService.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;


namespace CartingService.API.Controllers
{
    public partial class CartController : ControllerBase
    {
        /// <summary>
        ///  Get cart info.
        /// </summary>
        /// <param name="key">cart unique Key</param>
        /// <returns>a cart model(cart key + list of cart items)</returns>
        [MapToApiVersion("2.0")]
        [HttpGet("{key}", Name = "GetByCartId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<CartModel> GetListWithItems(string key)
        {
            var items = cartService.GetCartItems(Convert.ToInt32(key));

            return Ok(items.Select(x => mapper.Map<ItemModel>(x)).ToList());
        }
    }
}
