using AutoMapper;
using CartingService.API.Models;
using CartingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CartingService.API.Profiles
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            CreateMap<CartItem, ItemModel>()
                    .ReverseMap();
        }      
    }
}
