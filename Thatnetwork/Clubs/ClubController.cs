using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thatnetwork.Clubs.Dtos;
using Thatnetwork.Middlewares;
using Thatnetwork.Notes.Dtos;
using Thatnetwork.Notes;
using Thatnetwork.Users;

namespace Thatnetwork.Clubs
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubController : ControllerBase
    {
        private readonly ClubService _clubService;

        public ClubController(ClubService clubService)
        {
            _clubService = clubService;
        }

        [HttpPost("add-club"), Authorize()]
        public async Task<ActionResult> AddClub(AddClubDto addClubDto)
        {
            ClubDto? clubDto = await _clubService.GetClubByTagAsync(addClubDto.Tag);
            if (clubDto != null)
            {
                return UnprocessableEntity($"Клубный тег занят");
            }

            User currUser = HttpContext.GetCurrentUser();

            await _clubService.AddClubAsync(addClubDto, currUser);
            return Ok();
        }

        [HttpGet()]
        public async Task<ActionResult<List<ClubDto>>> GetAllClubs()
        {
            return await _clubService.GetAllClubsAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClubDto?>> GetNoteById(int id)
        {
            ClubDto? clubDto = await _clubService.GetClubByIdAsync(id);
            if (clubDto == null)
            {
                return NotFound();
            }

            return Ok(clubDto);
        }
    }
}
