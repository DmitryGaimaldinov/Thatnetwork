using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Thatnetwork.Dtos;
using Thatnetwork.Entities;

namespace Thatnetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto, ApplicationContext db)
        {
            if (db.Users.Any(u => u.Email == registerDto.Email))
            {
                return UnprocessableEntity("E-mail занят");
            }
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            var newUser = new User
            {
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                Username = registerDto.Username
            };

            db.Users.Add(newUser);
            await db.SaveChangesAsync();
            var allUsers = from users in db.Users select users;
            return Ok(allUsers.ToList());
        }

        [HttpPost("login-by-email")]
        public ActionResult<LoginResultDto> LoginByEmail([FromBody] LoginByEmailDto loginDto, ApplicationContext db)
        {
            var user = db.Users.Where(u => u.Email.Equals(loginDto.Email)).FirstOrDefault();
            if (user == null)
            {
                return UnprocessableEntity("Нет пользователя с таким e-mail");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return BadRequest("Неверный пароль");
            }

            LoginResultDto tokens = CreateTokens(user);

            return Ok(tokens);
        }

        [HttpPost("login-by-tag")]
        public ActionResult<LoginResultDto> LoginByTag([FromBody] LoginByTagDto loginDto, ApplicationContext db)
        {
            User? user;
            if (loginDto.Tag.StartsWith("id"))
            {
                try
                {
                    int id = int.Parse(loginDto.Tag.Substring(2));
                    user = db.Users.Where(u => u.Id.Equals(id)).FirstOrDefault();
                } catch
                {
                    return UnprocessableEntity("Некорректный тег");
                }
            } else
            {
                user = db.Users.Where(u => u.Tag.Equals(loginDto.Tag)).FirstOrDefault();
            }
            
            if (user == null)
            {
                return UnprocessableEntity("Нет пользователя с таким тегом");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return BadRequest("Неверный пароль");
            }

            var tokens = CreateTokens(user);

            return Ok(tokens);
        }

        private LoginResultDto CreateTokens(User user)
        {
            List<Claim> claims = new() { 
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

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

            return new LoginResultDto { accessToken = accessJwt, refreshToken = refreshJwt };
        }
    }
}
