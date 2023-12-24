using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Thatnetwork.Auth.Dtos;
using Thatnetwork.Dtos;
using Thatnetwork.Entities;
using Thatnetwork.Users;
using Thatnetwork.Users.Dtos;

namespace Thatnetwork.Auth
{


    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly UserService _userService;
        private readonly IMapper _mapper;

        public AuthController(
            IConfiguration configuration, 
            ILogger<AuthController> logger, 
            AppDbContext dbContext, 
            UserService userService,
            IMapper mapper
        )
        {
            _configuration = configuration;
            _logger = logger;
            _userService = userService;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (_dbContext.Users.Any(u => u.Email == registerDto.Email))
            {
                return UnprocessableEntity("Этот e-mail занят");
            }
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            var newUser = new User
            {
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                Name = registerDto.Username
            };

            await _userService.CreateUserAsync(newUser);
            var allUsers = _dbContext.Users.Include(u => u.RefreshTokens).ToList();

            //_logger.LogInformation(allUsers.ToString());
            return Ok(allUsers);
        }

        [HttpPost("login-by-email")]
        public async Task<ActionResult<LoginResultDto>> LoginByEmail([FromBody] LoginByEmailDto loginDto, ILogger<AuthController> logger)
        {
            User? user = _dbContext.Users.Where(u => u.Email.Equals(loginDto.Email)).SingleOrDefault();
            if (user == null)
            {
                return UnprocessableEntity("Нет пользователя с таким e-mail");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return UnprocessableEntity("Неверный пароль");
            }
            logger.LogInformation($"Adding user id to claims: {user.Id}");
            LoginResultDto tokens = await _CreateLoginResultDto(user);

            return Ok(tokens);
        }

        [HttpPost("login-by-tag")]
        public async Task<ActionResult<LoginResultDto>> LoginByTag([FromBody] LoginByTagDto loginDto)
        {
            User? user = _dbContext.Users.Where(u => u.Tag.Equals(loginDto.Tag)).FirstOrDefault();
            if (loginDto.Tag.StartsWith("id"))
            {
                try
                {
                    int id = int.Parse(loginDto.Tag.Substring(2));
                    user = _dbContext.Users.Where(u => u.Id.Equals(id)).FirstOrDefault();
                }
                catch
                {
                    return UnprocessableEntity("Некорректный тег");
                }
            }
            else
            {
                user = _dbContext.Users.Where(u => u.Tag.Equals(loginDto.Tag)).FirstOrDefault();
            }

            if (user == null)
            {
                return UnprocessableEntity("Нет пользователя с таким тегом");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return BadRequest("Неверный пароль");
            }

            var tokens = await _CreateLoginResultDto(user);

            return Ok(tokens);
        }

        [HttpPost("refresh-tokens")]
        public async Task<ActionResult<LoginResultDto>> RefreshTokens([FromBody] RefreshTokensDto refreshDto)
        {
            var jwtToken = new JwtSecurityToken(refreshDto.RefreshToken);
            string sub = jwtToken.Claims
                .First(c => c.Type.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase))
                .Value;
            int userId = int.Parse(sub);

            User user = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Include(u => u.RefreshTokens)
                .FirstAsync();
            RefreshToken? refreshEntity = user.RefreshTokens
                .FirstOrDefault(refreshEntity => refreshEntity.Token == refreshDto.RefreshToken);
            if (refreshEntity == null)
            {
                return Unauthorized();
            }
            user.RefreshTokens.Remove(refreshEntity);
            LoginResultDto newTokens = await _CreateLoginResultDto(user);
            await _dbContext.SaveChangesAsync();
            return newTokens;
        }

        private async Task<LoginResultDto> _CreateLoginResultDto(User user)
        {
            List<Claim> claims = new() {
                //new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                //new Claim(ClaimTypes.Role, "Admin")
            };

            /*var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("Token").Value!));*/

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ycyo89m3d6f94zyks763fp5qmgry4qgl"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var accessToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );
            var accessJwt = new JwtSecurityTokenHandler().WriteToken(accessToken);

            var refreshToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: creds
            );
            var refreshJwt = new JwtSecurityTokenHandler().WriteToken(refreshToken);
            user = await _dbContext.Users
                .Where(u => u.Id == user.Id)
                .Include(u => u.RefreshTokens)
                .FirstAsync();
            user.RefreshTokens.Add(new RefreshToken() { Token = refreshJwt, User = user });

            await _dbContext.SaveChangesAsync();

            return new LoginResultDto {
                AccessToken = accessJwt,
                RefreshToken = refreshJwt,
                UserDto = (await _userService.GetUserById(user.Id))!,
            };
        }
    }
}
