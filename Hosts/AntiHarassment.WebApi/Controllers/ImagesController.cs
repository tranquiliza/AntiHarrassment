using AntiHarassment.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AntiHarassment.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IFileRepository fileRepository;

        public ImagesController(IFileRepository fileRepository)
        {
            this.fileRepository = fileRepository;
        }

        [HttpGet("{imageName}")]
        public async Task<IActionResult> GetImage([FromRoute] string imageName)
        {
            var image = await fileRepository.GetImage(imageName).ConfigureAwait(false);

            var extension = Path.GetExtension(imageName);
            return File(image, ContentType(extension));
        }

        private string ContentType(string extension)
        {
            return extension switch
            {
                ".jpg" => "image/jpeg",
                ".png" => "image/png",
                _ => "image/jpeg",
            };
        }
    }
}
