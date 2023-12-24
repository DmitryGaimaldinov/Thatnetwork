using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thatnetwork.Middlewares;
using Thatnetwork.Photos;
using Thatnetwork.Users.Dtos;

namespace Thatnetwork.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly PhotoService _photoService;
        private IWebHostEnvironment _hostingEnvironment;

        public UserController(UserService userService, IWebHostEnvironment hostingEnvironment, PhotoService photoService)
        {
            _userService = userService;
            _hostingEnvironment = hostingEnvironment;
            _photoService = photoService;
            //_mediator = mediator;
        }

        [HttpGet("{tagOrId}")]
        public async Task<ActionResult<UserDto>> GetUserByTag(string tagOrId)
        {
            int? id = int.TryParse(tagOrId, out int userId) ? userId : null;
            UserDto? userDto;
            if (id == null)
            {
                userDto = await _userService.GetUserByTag(tagOrId);
            }
            else
            {
                userDto = await _userService.GetUserById((int)id);
            }
            
            if (userDto == null)
            {
                return NotFound();
            }
            return Ok(userDto);
        }

        [HttpPut("update-user"), Authorize]
        public async Task<ActionResult> UpdateUser([FromBody] UpdateUserDto dto)
        {
            User currUser = HttpContext.GetCurrentUser();
            if (dto.IsNotEmpty)
            {
                await _userService.UpdateUserAsync(currUser, dto);
            }
            return Ok();
        }

        [HttpPut("update-avatar"), Authorize]
        public async Task<ActionResult> UpdateAvatar(
            [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })] IFormFile file)
        {
            User currUser = HttpContext.GetCurrentUser();
            string photoPath = await _photoService.SavePhotoAsync(file, new SavePhotoSize { Height = 1080, Width = 1080 });
            await _userService.UpdateAvatarAsync(currUser, photoPath);

            return Ok();
        }

        // Для бэкграунда: 1080 x 432
    }
}
