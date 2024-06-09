using DiscountGrpcService.Protos;
using DiscountGrpcService.Repository;
using Grpc.Core;

namespace DiscountGrpcService.Services
{
    public class DiscountServiceGrpc : DiscountGrpcServiceProto.DiscountGrpcServiceProtoBase
    {
        private readonly IDiscountService _discountService;

        public DiscountServiceGrpc(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        public override Task<ResultAddDiscount> AddDiscount(RequestAddDiscount request, ServerCallContext context)
        {
            var data = _discountService.AddNewDiscount(request.Code, request.Amount);
            return Task.FromResult(new ResultAddDiscount
            {
                IsSuccess = true,
            });
        }

        public override Task<ResultGetDiscountByCode> GetDiscountByCode(RequestGetDiscountByCode request,
            ServerCallContext context)
        {
            var data = _discountService.GetDiscountByCode(request.Code);
            if (data == null)
            {
                return Task.FromResult(new ResultGetDiscountByCode
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "کد تخفیف پیدا نشد"
                });
            }

            return Task.FromResult(new ResultGetDiscountByCode
            {
                Data = new DiscountInfo
                {
                    Amount = data.Result.Amount,
                    Code = data.Result.Code,
                    Id = data.Result.Id.ToString(),
                    Used = data.Result.Used,
                },
                IsSuccess = true,
                Message = "اطلاعات تخفیف",
            });
        }

        public override Task<ResultUseDiscount> UseDiscount(RequestUseDiscount request, ServerCallContext context)
        {
            var dataBool = _discountService.UseDiscount(Guid.Parse(request.Id));
            return Task.FromResult(new ResultUseDiscount
            {
                IsSuccess = dataBool,
            });
        }

        public override Task<ResultGetDiscountByCode> GetDiscountById(RequestGetDiscountById request,
            ServerCallContext context)
        {
            var data = _discountService.GetDiscountById(Guid.Parse(request.Id));
            if (data == null)
            {
                return Task.FromResult(new ResultGetDiscountByCode
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "کد تخفیف پیدا نشد"
                });
            }

            return Task.FromResult(new ResultGetDiscountByCode
            {
                Data = new DiscountInfo
                {
                    Amount = data.Result.Amount,
                    Code = data.Result.Code,
                    Id = data.Result.Id.ToString(),
                    Used = data.Result.Used,
                },
                IsSuccess = true,
                Message = "اطلاعات تخفیف"
            });
        }
        
    }
}