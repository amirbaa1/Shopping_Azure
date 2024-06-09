using BasketService.Model.DTO;
using BasketService.Model.DTO.Discount;
using DiscountGrpcService.Protos;
using Grpc.Net.Client;

namespace BasketService.Repository.Discount
{
    public class DiscountService : IDiscountService
    {
        private readonly GrpcChannel _channel;

        public DiscountService(IConfiguration configuration)
        {
            _channel = GrpcChannel.ForAddress(configuration["AddressHttp:DiscountUrl"]);
        }

        public ResultDto<DiscountDto> GetDiscountByCode(string code)
        {
            var grpc_disc = new DiscountGrpcServiceProto.DiscountGrpcServiceProtoClient(_channel);
            var result = grpc_disc.GetDiscountByCode(new RequestGetDiscountByCode { Code = code });

            if (result.IsSuccess)
            {
                return new ResultDto<DiscountDto>
                {
                    Data = new DiscountDto
                    {
                        Amount = result.Data.Amount,
                        Code = result.Data.Code,
                        Id = Guid.Parse(result.Data.Id),
                        Used = result.Data.Used
                    },
                    IsSuccess = result.IsSuccess,
                    Message = result.Message
                };
            }

            return new ResultDto<DiscountDto>
            {
                IsSuccess = false,
                Message = result.Message
            };
        }

        public DiscountDto GetDiscountById(Guid discountId)
        {
            var grpcSer = new DiscountGrpcServiceProto.DiscountGrpcServiceProtoClient(_channel);
            var result = grpcSer.GetDiscountById(new RequestGetDiscountById
            {
                Id = discountId.ToString()
            });

            if (result.IsSuccess)
            {
                return new DiscountDto
                {
                    Amount = result.Data.Amount,
                    Code = result.Data.Code,
                    Id = Guid.Parse(result.Data.Id),
                    Used = result.Data.Used,
                };
            }
            return null;
        }
    }
}
