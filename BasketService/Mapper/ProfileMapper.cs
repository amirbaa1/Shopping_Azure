using AutoMapper;
using BasketService.Model.DTO;
using BasketService.Model;
using BasketService.Model.MessageDto;

namespace BasketService.Mapper
{
    public class ProfileMapper : Profile
    {
        public ProfileMapper()
        {
            CreateMap<BasketItem, AddItemToBasketDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<AddItemToBasketDto, ProductDto>().ReverseMap();
            CreateMap<CheckOutBasketDto, BasketCheckoutMessage>().ReverseMap();
        }
    }
}