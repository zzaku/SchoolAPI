using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public sealed class UserNotFoundException : NotFoundException
    {
        private int id;

        public UserNotFoundException(Guid accountId)
            : base($"The account with the identifier {accountId} was not found.")
        {
        }

        public UserNotFoundException(int id)
        {
            this.id = id;
        }
    }
}
