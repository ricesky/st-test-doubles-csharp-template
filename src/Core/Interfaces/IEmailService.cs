namespace Core.Interfaces;

public interface IEmailService
{
    void SendEmail(string address, string message);
}