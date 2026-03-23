using Microsoft.EntityFrameworkCore;
using QuantityMeasurementModel.Context;
using QuantityMeasurementModel.Entities;
using QuantityMeasurementRepository.Interface;

namespace QuantityMeasurementRepository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly QuantityMeasurementDbContext _context;

        public UserRepository(QuantityMeasurementDbContext context)
            => _context = context;

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
            => await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

        public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
            => await _context.Users.AnyAsync(u => u.Username == username, cancellationToken);

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
            => await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);

        public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }
    }
}
