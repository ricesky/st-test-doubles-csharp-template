namespace Core.Interfaces;

public interface IPaymentGateway
{
    bool Charge(decimal amount);
}
