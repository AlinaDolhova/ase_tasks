using AutoMapper;
using CatalogService.DAL.Models;
using CatalogService.GraphQL.Models;

namespace CatalogService.GraphQL.MapperProfiles
{
    public class GraphQlMappingProfile : Profile
    {
        public GraphQlMappingProfile()
        {
            CreateMap<Model.Item, ItemViewModel>()
                .ReverseMap();

            CreateMap<Model.Category, CategoryViewModel>()
                .ForMember(x => x.Items, y => y.MapFrom(z => z.Items))
                .ReverseMap();

            CreateMap<Model.Category, IdNamePair>();
            CreateMap<Model.Item, IdNamePair>();

            CreateMap<CategoryInput, Model.Category>();
            CreateMap<ItemInput, Model.Item>();
        }
    }
}
