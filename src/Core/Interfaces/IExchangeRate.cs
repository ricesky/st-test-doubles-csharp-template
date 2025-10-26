namespace Core.Interfaces;

public interface IExchangeRate
{
    decimal GetRate(string currency);
}