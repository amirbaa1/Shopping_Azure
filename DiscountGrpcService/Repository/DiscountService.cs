using DiscountGrpcService.Data;
using DiscountGrpcService.Model;
using DiscountGrpcService.Model.DTO;
using MongoDB.Driver;

namespace DiscountGrpcService.Repository;

public class DiscountService : IDiscountService
{
    private readonly ILogger<DiscountService> _logger;
    private readonly IDiscountContext _context;

    public DiscountService(ILogger<DiscountService> logger, IDiscountContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<DiscountDto> GetDiscountByCode(string code)
    {
        var getDisc = await _context.discounts.Find(x => x.Code == code).FirstOrDefaultAsync();
        if (getDisc == null)
        {
            return null;
        }

        var result = new DiscountDto
        {
            Amount = getDisc.Amount,
            Code = getDisc.Code,
            Id = getDisc.Id,
            Used = getDisc.Used
        };
        return result;
    }

    public async Task<DiscountDto> GetDiscountById(Guid id)
    {
        var getDis = await _context.discounts.Find(x => x.Id == id).FirstOrDefaultAsync();
        if (getDis == null)
        {
            return null;
        }

        var result = new DiscountDto
        {
            Amount = getDis.Amount,
            Code = getDis.Code,
            Id = getDis.Id,
            Used = getDis.Used
        };
        return result;
    }

    public bool UseDiscount(Guid Id)
    {
        var getDisc = _context.discounts.Find(x => x.Id == Id).FirstOrDefault();
        if (getDisc == null)
            throw new Exception("Discouint Not Found....");

        getDisc.Used = true;
        var filter = Builders<DiscountCode>.Filter.Eq(x => x.Id, Id);
        _context.discounts.ReplaceOne(filter,getDisc);
        return true;
    }

    public bool AddNewDiscount(string Code, int Amount)
    {
        var discount = new DiscountCode()
        {
            Amount = Amount,
            Code = Code,
            Used = false,
        };
        _context.discounts.InsertOneAsync(discount);
        return true;
    }
}