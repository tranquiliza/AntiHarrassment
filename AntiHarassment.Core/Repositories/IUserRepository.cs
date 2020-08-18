using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface IUserRepository
    {
        Task<User> GetByTwitchUsername(string email);
        Task Save(User user);
        Task<User> GetById(Guid id);
        Task Delete(Guid userId);
    }
}
