using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public sealed class UserDoesNotBelongToOwnerException : BadRequestException
    {
        public UserDoesNotBelongToOwnerException(Guid ownerId, Guid accountId)
            : base($"The account with the identifier {accountId} does not belong to the owner with the identifier {ownerId}")
        {
        }
    }
}
