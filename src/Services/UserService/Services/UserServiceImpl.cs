using UserService.Data;
using UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace UserService.Services
{
    public class UserServiceImpl : IUserService
    {
        private readonly UserDbContext _db;
        public UserServiceImpl(UserDbContext db) { _db = db; }

        public async Task<User> RegisterAsync(string email, string password)
        {
            var existing = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (existing != null) throw new Exception("User already exists");

            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;

        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null) return null;
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;
            return user;
        }
    }
}
