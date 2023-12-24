using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Processing;
using Thatnetwork.Entities;
using Thatnetwork.Users;

namespace Thatnetwork.Photos
{
    public class PhotoService
    {
        private IWebHostEnvironment _hostingEnvironment;
        private AppDbContext _dbContext;
        
        public PhotoService(IWebHostEnvironment hostingEnvironment, AppDbContext dbContext)
        {
            _hostingEnvironment = hostingEnvironment;
            _dbContext = dbContext;
        }

        public Photo? GetPhotoById(int id)
        {
            return _dbContext.Photos.Include(p => p.Owner).SingleOrDefault(p => p.Id == id);
        }

        public async Task<int> AddPhotoAsync(string path, User owner)
        {
            Photo photo = new Photo { Owner  = owner, Path = path };
            _dbContext.Photos.Add(photo);
            await _dbContext.SaveChangesAsync();
            photo = await _dbContext.Photos.SingleAsync(p => p.Path == path);
            return photo.Id;
        }

        public async Task<string> SavePhotoAsync(IFormFile file, SavePhotoSize? size = null)
        {
            //string photosPath = Path.Combine(_hostingEnvironment.WebRootPath, "photos");
            string photosPath = _hostingEnvironment.WebRootPath;
            string filePath = Path.Combine(photosPath, $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");

            using var image = Image.Load(file.OpenReadStream());

            if (size != null)
            {
                //image.Mutate(i => i.Crop(size.Width, size.Height));
                image.Mutate(i => i.Resize(new ResizeOptions
                {
                    Size = new Size(size.Width, size.Height),
                    Mode = ResizeMode.Crop,
                    Position = AnchorPositionMode.Center,
                }));
            }
            await image.SaveAsync(filePath);

            //Photo photo = new Photo { Path = Path.GetFileName(filePath), Owner = owner };
            //_dbContext.Photos.Add(photo);
            //await _dbContext.SaveChangesAsync();

            return Path.GetFileName(filePath);
        }
    }
}
