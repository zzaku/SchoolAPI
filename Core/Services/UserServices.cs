using Domain.Entities;
using Services.Abstractions;
using Domain.Exceptions;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Services
{
    internal sealed class UserServices : IUserService
    {
        private readonly IRepositoryManager _repositoryManager;

        public UserServices(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager ?? throw new ArgumentNullException(nameof(repositoryManager));
        }

        public async Task<IEnumerable<User>> GetAllByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
        {
            var owner = await _repositoryManager.GetUserByIdAsync(ownerId, cancellationToken);
            if (owner == null)
            {
                throw new OwnerNotFoundException(ownerId);
            }

            // Récupérez tous les utilisateurs associés à ce propriétaire.
            var users = await _repositoryManager
                .Find(u => u.OwnerId == owner.Id)
                .ToListAsync(cancellationToken);

            return users;
        }

        public async Task<User> GetByIdAsync(Guid ownerId, Guid userId, CancellationToken cancellationToken)
        {
            var owner = await _repositoryManager.GetUserByIdAsync(ownerId, cancellationToken);
            if (owner == null)
            {
                throw new OwnerNotFoundException(ownerId);
            }

            var user = await _repositoryManager
                .Find(u => u.Id == userId && u.OwnerId == owner.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }

            return user;
        }

        public async Task<User> CreateUserAsync(Guid ownerId, User user, CancellationToken cancellationToken = default)
        {
            var owner = await _repositoryManager.GetUserByIdAsync(ownerId, cancellationToken);
            if (owner == null)
            {
                throw new OwnerNotFoundException(ownerId);
            }

            user.OwnerId = owner.Id;

            _repositoryManager.Insert(user);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

            return user; // Retournez l'utilisateur créé.
        }

        public async Task<User> UpdateUserAsync(Guid ownerId, User user, CancellationToken cancellationToken = default)
        {
            var owner = await _repositoryManager.GetUserByIdAsync(ownerId, cancellationToken);
            if (owner == null)
            {
                throw new OwnerNotFoundException(ownerId);
            }

            // Vous devez obtenir l'utilisateur existant à mettre à jour à partir du DbContext.
            var existingUser = await _repositoryManager.UserRepository
                .FirstOrDefaultAsync(u => u.Id == user.Id && u.OwnerId == owner.Id, cancellationToken);

            if (existingUser == null)
            {
                throw new UserNotFoundException(user.Id);
            }

            // Mettez à jour les propriétés de l'utilisateur existant avec les nouvelles valeurs.
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.Password = user.Password;

            // Vous pouvez également mettre à jour d'autres propriétés si nécessaire.

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

            return existingUser;
        }

        public async Task DeleteUserAsync(Guid ownerId, Guid userId, CancellationToken cancellationToken = default)
        {
            var owner = await _repositoryManager.OwnerRepository.GetByIdAsync(ownerId, cancellationToken);
            if (owner == null)
            {
                throw new OwnerNotFoundException(ownerId);
            }

            var user = await _repositoryManager.UserRepository
                .Find(u => u.Id == userId && u.OwnerId == owner.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }

            _repositoryManager.UserRepository.Remove(user);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
