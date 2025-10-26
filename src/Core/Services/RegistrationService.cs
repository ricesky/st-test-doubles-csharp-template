using Core.Interfaces;
using Core.Models;

namespace Core.Services;
public class RegistrationService
{
    private readonly IUserRepository _repo;
    public RegistrationService(IUserRepository repo) => _repo = repo;

    public bool Register(User user)
    {
        if (_repo.Find(user.Username) is not null) return false;
        _repo.Add(user);
        return true;
    }
}
