using Microsoft.EntityFrameworkCore;
using StudyConnect.API.Data;
using StudyConnect.API.Models;
using StudyConnect.API.Repositories.Interfaces;

namespace StudyConnect.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly StudyConnectDbContext _context;

    public UserRepository(StudyConnectDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
        => await _context.Users.FindAsync(id);

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
