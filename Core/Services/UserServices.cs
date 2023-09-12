using Domain.Exceptions;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Services.Abstractions;
using MediaBrowser.Model.Dto;

namespace Services
{
    internal sealed class UserServices : IUserService
    {
        private readonly IRepositoryManager _repositoryManager;

        public UserServices(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager ?? throw new ArgumentNullException(nameof(repositoryManager));
        }

        public async Task<IEnumerable<UserDto>> GetAllByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
        {
            var users = await _repositoryManager.UserRepository.GetAllByOwnerIdAsync(ownerId, cancellationToken);
            var userDtos = users.Adapt<IEnumerable<UserDto>>();
            return userDtos;
        }

        public async Task<UserDto> GetByIdAsync(Guid ownerId, Guid userId, CancellationToken cancellationToken)
        {
            var owner = await _repositoryManager.OwnerRepository.GetByIdAsync(ownerId, cancellationToken);
            if (owner is null)
            {
                throw new OwnerNotFoundException(ownerId);
            }

            var user = await _repositoryManager.UserRepository.GetByIdAsync(userId, cancellationToken);
            if (user is null)
            {
                throw new UserNotFoundException(userId);
            }

            if (user.OwnerId != owner.Id)
            {
                throw new UserDoesNotBelongToOwnerException(owner.Id, user.Id);
            }

            var userDto = user.Adapt<UserDto>();
            return userDto;
        }

        public async Task<UserDto> CreateAsync(Guid ownerId, UserForCreationDto userForCreationDto, CancellationToken cancellationToken = default)
        {
            var owner = await _repositoryManager.OwnerRepository.GetByIdAsync(ownerId, cancellationToken);
            if (owner is null)
            {
                throw new OwnerNotFoundException(ownerId);
            }

            var user = userForCreationDto.Adapt<User>();
            user.OwnerId = owner.Id;

            _repositoryManager.UserRepository.Insert(user);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

            var userDto = user.Adapt<UserDto>();
            return userDto;
        }

        public async Task DeleteAsync(Guid ownerId, Guid userId, CancellationToken cancellationToken = default)
        {
            var owner = await _repositoryManager.OwnerRepository.GetByIdAsync(ownerId, cancellationToken);
            if (owner is null)
            {
                throw new OwnerNotFoundException(ownerId);
            }

            var user = await _repositoryManager.UserRepository.GetByIdAsync(userId, cancellationToken);
            if (user is null)
            {
                throw new UserNotFoundException(userId);
            }

            if (user.OwnerId != owner.Id)
            {
                throw new UserDoesNotBelongToOwnerException(owner.Id, user.Id);
            }

            _repositoryManager.UserRepository.Remove(user);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}