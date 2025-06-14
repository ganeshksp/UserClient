using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserClient.Core.Models;

namespace UserClient.Core.Interfaces
{
    public interface IExternalUserService
    {
        /// <summary>
        /// Fetches all users (across all pages).
        /// </summary>
        Task<IEnumerable<User>> GetAllUsersAsync();

        /// <summary>
        /// Fetches a single user by their ID, or null if not found.
        /// </summary>
        Task<User?> GetUserByIdAsync(int userId);
    }
}
