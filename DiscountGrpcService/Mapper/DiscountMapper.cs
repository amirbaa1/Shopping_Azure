using AutoMapper;
using DiscountGrpcService.Model;
using DiscountGrpcService.Model.DTO;

namespace DiscountGrpcService.Mapper
{
    public class DiscountMapper : Profile
    {
        public DiscountMapper()
        {
            CreateMap<DiscountCode, DiscountDto>().ReverseMap();
        }
    }
}
