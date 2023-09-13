using Domain.Entities;

namespace Services.Abstractions
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
        Task<User> GetByIdAsync(Guid ownerId, Guid userId, CancellationToken cancellationToken);
        Task<User> CreateUserAsync(Guid ownerId, User user, CancellationToken cancellationToken = default);
        Task<User> UpdateUserAsync(Guid ownerId, Guid userId, User updatedUser, CancellationToken cancellationToken = default);
        Task DeleteUserAsync(Guid ownerId, Guid userId, CancellationToken cancellationToken = default);
    }
}
