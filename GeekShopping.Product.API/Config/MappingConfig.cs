using AutoMapper;
using GeekShopping.Product.API.Data.ValueObjects;
using GeekShopping.Product.API.Models;

namespace GeekShopping.Product.API.Config
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductsVO, Products>();
                config.CreateMap<Products, ProductsVO>();
            });
            return mappingConfig;
        }
    }
}
