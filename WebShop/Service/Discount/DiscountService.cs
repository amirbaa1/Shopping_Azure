using DiscountGrpcService.Protos;
using Grpc.Net.Client;
using WebShop.Model.Discount.DTO;
using WebShop.Model.DTO;

namespace WebShop.Service.Discount;

public class DiscountService : IDiscountService
{
    private readonly GrpcChannel _grpcChannel;
    private readonly IConfiguration _configuration;

    public DiscountService(IConfiguration configuration)
    {
        _configuration = configuration;
        _grpcChannel = GrpcChannel.ForAddress(configuration["AddressHttp:DiscountUrl"]!);
    }


    public ResultDto<DiscountDto> GetDiscountByCode(string code)
    {
        var grpc_disc = new DiscountGrpcServiceProto.DiscountGrpcServiceProtoClient(_grpcChannel);
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

    public ResultDto<DiscountDto> GetDiscountById(Guid id)
    {
        var grpcSer = new DiscountGrpcServiceProto.DiscountGrpcServiceProtoClient(_grpcChannel);
        var result = grpcSer.GetDiscountById(new RequestGetDiscountById { Id = id.ToString() });
        if (result == null)
        {
            return new ResultDto<DiscountDto>
            {
                IsSuccess = false,
                Message = result.Message
            };
        }

        return new ResultDto<DiscountDto>
        {
            Data = new DiscountDto
            {
                Amount = result.Data.Amount,
                Code = result.Data.Code,
                Used = result.Data.Used
            },
            IsSuccess = result.IsSuccess,
            Message = result.Message
        };
    }

    public ResultDto UseDiscount(Guid discountId)
    {
        var grpcSer = new DiscountGrpcServiceProto.DiscountGrpcServiceProtoClient(_grpcChannel);
        var result = grpcSer.UseDiscount(new RequestUseDiscount { Id = discountId.ToString() });
        if (result.IsSuccess)
        {
            return new ResultDto
            {
                IsSuccess = result.IsSuccess
            };
        }

        return new ResultDto
        {
            IsSuccess = false,
        };
    }
}