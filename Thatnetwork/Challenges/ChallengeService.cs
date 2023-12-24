using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks.Dataflow;
using System.Web.Http;
using Thatnetwork.Challenges.Dtos;
using Thatnetwork.Challenges.Events;
using Thatnetwork.Chats;
using Thatnetwork.Entities;
using Thatnetwork.Extensions;
using Thatnetwork.Photos;
using Thatnetwork.Users;
using Thatnetwork.Users.Dtos;

namespace Thatnetwork.Challenges
{
    public class ChallengeService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;

        public ChallengeService(
            AppDbContext dbContext, 
            IMapper mapper, 
            IMediator mediator, IServiceProvider serviceProvider) 
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _mediator = mediator;
            _serviceProvider = serviceProvider;
        }

        public async Task<MarathonDto?> GetMaraphonByIdAsync(int id, User? user)
        {
            Marathon? marathon = await _dbContext.Marathons.SingleOrDefaultAsync(m => m.Id == id);
            if (marathon == null)
            {
                return null;
            }
            return MapMarathonToDto(marathon, user?.Id);
        }

        public async Task<MarathonDto?> GetMaraphonByTagAsync(string tag, User? user)
        {
            var marathon = await _dbContext.Marathons.SingleOrDefaultAsync(m => m.Tag == tag);
            if (marathon == null)
            {
                return null;
            }
            return MapMarathonToDto(marathon, user?.Id);
        }

        public async Task<List<MarathonDto>> GetAllMarathonsAsync(User? user)
        {
            var marathons = await _dbContext.Marathons.ToListAsync();
            return marathons.Select(m => MapMarathonToDto(m, user?.Id)).ToList();
        }

        public async Task<MarathonDto> AddMaraphonAsync(AddMarathonDto addMaraphonDto, User creator)
        {
            var chatRoom = new ChatRoom { 
                Name = addMaraphonDto.Name,
                Creator = creator,
                Participants = new List<User> { creator },
            };

            var marathon = new Marathon
            {
                Creator = creator,
                Description = addMaraphonDto.Description,
                EndDate = addMaraphonDto.EndDate,
                Name = addMaraphonDto.Name,
                StartDate = addMaraphonDto.StartDate.TrimSeconds(),
                Tag = addMaraphonDto.Tag,
                Participants = new List<User>() { creator },
                ChatRoom = chatRoom,
            };
            List<ChallengeHashtag> hashtags = new();
            foreach (var hashtag in addMaraphonDto.Hashtags)
            {
                ChallengeHashtag cHashtag = _dbContext.ChallengeHashtags
                    .SingleOrDefault(cHashtag => cHashtag.Text == hashtag.Trim()) 
                        ?? new ChallengeHashtag { Marathon = marathon, Text = hashtag };
                hashtags.Add(cHashtag);
            }
            marathon.Hashtags = hashtags;
            if (addMaraphonDto.AvatarPhotoId != null)
            {
                Photo photo = _dbContext.Photos.Single(p => p.Id == addMaraphonDto.AvatarPhotoId);
                marathon.Avatar = photo;
            }

            _dbContext.Marathons.Add(marathon);
            await _dbContext.SaveChangesAsync();

            MarathonDto marathonDto = _mapper.Map<MarathonDto>(marathon);
            // OnMarathonCreated(marathonDto);

            return marathonDto;
        }

        /*private void OnMarathonCreated(MarathonDto marathonDto)
        {
            Task
                // .Delay(marathonDto.StartDate.Subtract(DateTime.Now))
                .Delay(TimeSpan.FromSeconds(2))
                .ContinueWith((_) =>
                {
                    _mediator.Publish(new ChallengeStartedNotification() { MarathonDto = marathonDto });
                }).Start();
        }*/

        public async Task JoinMarathon(int marathonId, User user)
        {
            Marathon marathon = await _dbContext.Marathons
                .Include(m => m.Participants)
                .SingleAsync(m => m.Id == marathonId);
            marathon.Participants.Add(user);
            await _dbContext.SaveChangesAsync();
            // await _chatService.JoinChatRoomAsync();
        }

        public async Task LeaveMarathon(int marathonId, User user)
        {
            Marathon marathon = await _dbContext.Marathons
                .Include(m => m.Participants)
                .SingleAsync(m => m.Id == marathonId);
            marathon.Participants.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<UserDto>> GetParticipantsAsync(int marathonId)
        {
            Marathon marathon = await _dbContext.Marathons
                .Include(m => m.Participants)
                .SingleAsync(m => m.Id == marathonId);
            return _mapper.Map<List<UserDto>>(marathon.Participants);
        }

        public bool IsJoined(int marathonId, int userId)
        {
            bool isJoined = _dbContext.Marathons
                .Include(m => m.Participants)
                .Any(m => m.Id == marathonId && m.Participants.Any(p => p.Id == userId));
            return isJoined;
        }

        public async Task<List<MarathonDto>> GetUserChallenges(int userId)
        {
            var marathons = await _dbContext.Marathons
                .Include(m => m.Participants)
                .Where(m => m.Participants.Any(p => p.Id == userId))
                .ToListAsync();
            return marathons.Select(m => MapMarathonToDto(m, userId)).ToList();
        }

        public MarathonDto MapMarathonToDto(Marathon marathon, int? userId)
        {
            int participantsCount = _dbContext.Users
                .Include(u => u.TakenMaraphones)
                .Where(u => u.TakenMaraphones.Any(m => m.Id == marathon.Id))
                .Count();
            MarathonDto marathonDto = _mapper.Map<MarathonDto>(marathon);
            marathonDto.isJoined = (userId == null) ? false : IsJoined(marathon.Id, (int)userId);
            marathonDto.ParticipantsCount = participantsCount;
            return marathonDto;
        }

       

        // public async Task<bool> ParticipatedInMaraphon(int marathonId, User user)
        //{
        //    Marathon marathon = await _dbContext.Marathons.SingleAsync(m => m.Id == challengeId);
        //}
    }
}
