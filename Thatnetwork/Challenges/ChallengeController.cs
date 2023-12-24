using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thatnetwork.Clubs.Dtos;
using Thatnetwork.Clubs;
using Thatnetwork.Middlewares;
using Thatnetwork.Users;
using Microsoft.AspNetCore.Authorization;
using Thatnetwork.Challenges.Dtos;
using Thatnetwork.Users.Dtos;
using Thatnetwork.Photos;
using Thatnetwork.Challenges.Validators;
using MediatR;
using Thatnetwork.Chats.Dtos;
using Thatnetwork.Chats;
using Thatnetwork.Challenges.Events;

namespace Thatnetwork.Challenges
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChallengeController : ControllerBase
    {
        private readonly ChallengeService _challengeService;
        private readonly PhotoService _photoService;
        private readonly ILogger<ChallengeController> _logger;
        private readonly ChatService _chatService;
        private readonly IMediator _mediator;

        public ChallengeController(
            ChallengeService challengeService, 
            PhotoService photoService, 
            ILogger<ChallengeController> logger,
            ChatService chatService,
            IMediator mediator
        ) {
            _challengeService = challengeService;
            _photoService = photoService;
            _logger = logger;
            _chatService = chatService;
            _mediator = mediator;
        }

        [HttpGet()]
        public async Task<ActionResult<List<MarathonDto>>> GetAllChallenges()
        {
            User? currUser = HttpContext.GetCurrentUserOrNull();
            
            return await _challengeService.GetAllMarathonsAsync(currUser);
        }

        [HttpGet("{tag}")]
        public async Task<ActionResult<MarathonDto>> GetMarathonByTag(string tag)
        {
            User? currUser = HttpContext.GetCurrentUserOrNull();
            MarathonDto? marathonDto = await _challengeService.GetMaraphonByTagAsync(tag, currUser);

            if (marathonDto == null)
            {
                return NotFound();
            }
            return Ok(marathonDto);
        }

        //[HttpPut(), Authorize()]
        //public async Task<ActionResult<MarathonDto>> UpdateMarathon([FromBody] )
        //{
        //}

        [HttpPost("add-marathon"), Authorize()]
        public async Task<ActionResult<MarathonDto>> AddMarathon(AddMarathonDto addMarathonDto)
        {
            User currUser = HttpContext.GetCurrentUser();
            MarathonDto? marathonDto = await _challengeService.GetMaraphonByTagAsync(addMarathonDto.Tag, currUser);
            if (marathonDto != null)
            {
                return UnprocessableEntity($"Тег занят");
            }
            
            if (addMarathonDto.EndDate < addMarathonDto.StartDate)
            {
                return UnprocessableEntity("Дата конца не может быть меньше даты начала");
            }
            if (addMarathonDto.Hashtags.Any())
            {
                addMarathonDto.Hashtags = addMarathonDto.Hashtags
                    .Select(h => h.Trim().ToLower())
                    .Distinct()
                    .ToList();
            }

            MarathonDto createdMarathon = await _challengeService.AddMaraphonAsync(addMarathonDto, currUser);

            return Ok(createdMarathon);
        }


        [HttpPost("join"), Authorize]
        public async Task<ActionResult> JoinMarathon(JoinMarathonDto dto)
        {
            User currUser = HttpContext.GetCurrentUser();
            await _challengeService.JoinMarathon(dto.MarathonId, currUser);
            return Ok();
        }

        [HttpPost("leave"), Authorize]
        public async Task<ActionResult> LeaveMarathon(MarathonActionDataDto dto)
        {
            User currUser = HttpContext.GetCurrentUser();
            await _challengeService.LeaveMarathon(dto.MarathonId, currUser);
            return Ok();
        }

        [HttpPost("is-joined"), Authorize]
        public async Task<ActionResult> IsJoined(MarathonActionDataDto dto)
        {
            User currUser = HttpContext.GetCurrentUser();
            bool isJoined = _challengeService.IsJoined(dto.MarathonId, currUser.Id);
            return Ok(isJoined);
        }

        [HttpPost("get-participants")]
        public async Task<ActionResult<List<UserDto>>> GetParticipants(MarathonActionDataDto dto)
        {
            return Ok(await _challengeService.GetParticipantsAsync(dto.MarathonId));
        }

        [HttpPost("get-my-challenges"), Authorize]
        public async Task<ActionResult<List<UserDto>>> GetMyChallenges()
        {
            int currUserId = HttpContext.GetCurrentUser().Id;

            return Ok(await _challengeService.GetUserChallenges(currUserId));
        }
    }
}
