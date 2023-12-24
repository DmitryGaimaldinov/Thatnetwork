using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Thatnetwork.Clubs.Dtos;
using Thatnetwork.Entities;
using Thatnetwork.Users;

namespace Thatnetwork.Clubs
{
    public class ClubService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public ClubService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task AddClubAsync(AddClubDto addClubDto, User creator)
        {
            Club newClub = new Club { 
                Name = addClubDto.Name, 
                Tag = addClubDto.Tag, 
                Description = addClubDto.Description,
                Creator = creator,
                AvatarId = addClubDto.AvatarId,
            };
            _dbContext.Clubs.Add(newClub);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ClubDto>> GetAllClubsAsync()
        {
            var clubs = await _dbContext.Clubs.ToListAsync();
            return _mapper.Map<List<ClubDto>>(clubs);
        }

        public async Task<ClubDto?> GetClubByIdAsync(int id)
        {
            var club = await _dbContext.Clubs.FirstOrDefaultAsync(c => c.Id == id);
            return _mapper.Map<ClubDto?>(club);
        }

        public async Task<ClubDto?> GetClubByTagAsync(string tag)
        {
            var club = await _dbContext.Clubs.FirstOrDefaultAsync(c => c.Tag == tag);
            return _mapper.Map<ClubDto>(club);
        }
    }
}
