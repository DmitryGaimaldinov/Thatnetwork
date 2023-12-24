using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using Thatnetwork.Entities;
using Thatnetwork.Middlewares;
using Thatnetwork.Users;

namespace Thatnetwork.Photos
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly PhotoService _photoService;

        public PhotoController(PhotoService photoService)
        {
            _photoService = photoService;
        }

        [HttpPost("upload-photo"), Authorize]
        public async Task<ActionResult<int>> UploadPhoto([AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })] IFormFile file)
        {
            User currUser = HttpContext.GetCurrentUser();
            String photoPath = await _photoService.SavePhotoAsync(file);
            int photoId = await _photoService.AddPhotoAsync(photoPath, currUser);

            return Ok(photoId);
        }

        [HttpPost("upload-photos"), Authorize]
        public async Task<ActionResult<List<int>>> UploadPhotos([AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })] List<IFormFile> files)
        {
            User currUser = HttpContext.GetCurrentUser();
            List<int> photoIds = new();
            foreach (IFormFile file in files)
            {
                String photoPath = await _photoService.SavePhotoAsync(file);
                int photoId = await _photoService.AddPhotoAsync(photoPath, currUser);
                photoIds.Add(photoId);
            }
            return Ok(photoIds);
        }

        //public async Task<ActionResult> UploadPhotos(List<IFormFile> files)
        //{
        //foreach (var formFile in files)
        //{
        //    if (formFile.Length > 0)
        //    {
        //        var filePath = Path.Combine(_config["StoredFilesPath"],
        //        Path.GetRandomFileName());

        //        using (var stream = System.IO.File.Create(filePath))
        //        {
        //            await formFile.CopyToAsync(stream);
        //        }

        //        using (var stream = System.IO.File.Create(filePath))
        //        {
        //            await formFile.CopyToAsync(stream);
        //        }
        //    }
        //}

        //// Process uploaded files
        //// Don't rely on or trust the FileName property without validation.

        //return Ok();
        //}
    }
}
