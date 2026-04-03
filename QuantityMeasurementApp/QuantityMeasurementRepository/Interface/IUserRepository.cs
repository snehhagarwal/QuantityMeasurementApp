using QuantityMeasurementModel.Entities;

namespace QuantityMeasurementRepository.Interface
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User> AddAsync(User user, CancellationToken cancellationToken = default);

        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken ct = default);
        Task<User> UpdateAsync(User user, CancellationToken ct = default);
    }
}
