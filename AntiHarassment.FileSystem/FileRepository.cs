using AntiHarassment.Core;
using System.IO;
using System.Threading.Tasks;

namespace AntiHarassment.FileSystem
{
    public class FileRepository : IFileRepository
    {
        private readonly string imageStoragePath;

        public FileRepository(string fileStoragePath)
        {
            imageStoragePath = Path.Combine(fileStoragePath, "images");
        }

        public async Task SaveImage(byte[] imageData, string filename)
        {
            var filePath = Path.Combine(imageStoragePath, filename);
            Directory.CreateDirectory(imageStoragePath);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await fileStream.WriteAsync(imageData);
        }

        public async Task<byte[]> GetImage(string fileName)
        {
            var imagePath = Path.Combine(imageStoragePath, fileName);

            return await File.ReadAllBytesAsync(imagePath).ConfigureAwait(false);
        }
    }
}
