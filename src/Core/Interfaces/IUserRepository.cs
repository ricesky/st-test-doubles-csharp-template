using Core.Models;

namespace Core.Interfaces;

public interface IUserRepository
{
    void Add(User user);
    User? Find(string username);
}