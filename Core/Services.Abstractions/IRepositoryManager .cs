using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Services.Abstractions
{
    public interface IRepositoryManager
    {
        object UserRepository { get; }

        Task<IEnumerable<User>> GetAllUsersAsync(Guid ownerId, CancellationToken cancellationToken = default);
        Task<User> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task CreateUserAsync(User user, CancellationToken cancellationToken = default);
        Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
        Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Course>> GetAllCoursesAsync(CancellationToken cancellationToken = default);
        Task<Course> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken = default);
        Task CreateCourseAsync(Course course, CancellationToken cancellationToken = default);
        Task UpdateCourseAsync(Course course, CancellationToken cancellationToken = default);
        Task DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken = default);

    }
}
