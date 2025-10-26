using Core.Interfaces;

namespace Core.Services;

public class PriceService
{
    private readonly IExchangeRate _rate;
    public PriceService(IExchangeRate rate) => _rate = rate;
    public decimal ConvertToIDR(decimal usd) => usd * _rate.GetRate("USD");
}
