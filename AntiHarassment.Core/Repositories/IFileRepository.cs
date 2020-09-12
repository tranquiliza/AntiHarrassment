using System.Threading.Tasks;

namespace AntiHarassment.Core
{
    public interface IFileRepository
    {
        Task SaveImage(byte[] imageData, string filename);
        Task<byte[]> GetImage(string fileName);
    }
}
