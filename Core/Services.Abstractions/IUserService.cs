using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Dto;

namespace Services.Abstractions
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
        Task<UserDto> GetByIdAsync(Guid ownerId, Guid userId, CancellationToken cancellationToken);
        Task<UserDto> CreateAsync(Guid ownerId, UserForCreationDto userForCreationDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid ownerId, Guid userId, CancellationToken cancellationToken = default);
    }
}