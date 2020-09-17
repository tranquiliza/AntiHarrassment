using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface IDeletedMessagesRepository
    {
        Task Insert(string channel, string username, string deletedBy, string message, DateTime timestamp);
    }
}
