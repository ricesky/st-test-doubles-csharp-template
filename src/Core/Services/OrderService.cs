using Core.Interfaces;
using Core.Models;

namespace Core.Services;
public class OrderService
{
    private readonly IEmailService _email;
    private readonly IPaymentGateway _gateway;
    private readonly ILogger _logger;
    public OrderService(IEmailService email, IPaymentGateway gateway, ILogger logger)
    { _email = email; _gateway = gateway; _logger = logger; }

    public bool PlaceOrder(Order order)
    {
        var ok = _gateway.Charge(order.Total);
        if (!ok) { _logger.Log("Payment failed"); return false; }
        _email.SendEmail(order.CustomerEmail, "Order confirmed!");
        return true;
    }
}
