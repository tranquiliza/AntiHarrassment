using AntiHarassment.Contract.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.Frontend.Application
{
    public interface ITagService
    {
        event Action OnChange;
        List<TagModel> Tags { get; }

        Task Initialize();
        Task AddNewTag(string tagName);
    }
}
