using UserService.Models;

namespace UserService.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(string email, string password);
        Task<User?> AuthenticateAsync(string email, string password);
    }
}
