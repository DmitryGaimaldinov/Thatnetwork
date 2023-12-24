using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thatnetwork.Entities;
using Thatnetwork.Photos;
using Thatnetwork.Users.Dtos;
using Thatnetwork.Users.Notifications;

namespace Thatnetwork.Users
{
    public class UserService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public UserService(AppDbContext dbContext, IMapper mapper
            , IMediator mediator
        )
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task CreateUserAsync(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserDto?> GetUserById(int id)
        {
            User? userEntity = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == id);
            if (userEntity == null)
            {
                return null;
            }
            return await _MapUserToDtoAsync(userEntity);
        }

        private async Task<UserDto> _MapUserToDtoAsync(User user)
        {
            user = await _dbContext.Users
                .SingleAsync(u => u.Id == user.Id);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> GetUserByTag(string tag)
        {
            if (_IsIdentiedTag(tag))
            {
                int userId = _GetIdFromIdentiedTag(tag);
                return await GetUserById(userId);
            }

            User? userEntity = await _dbContext.Users.SingleOrDefaultAsync(u => u.Tag == tag);
            if (userEntity == null)
            {
                return null;
            }
            return await _MapUserToDtoAsync(userEntity);
        }

        public async Task UpdateUserAsync(User user, UpdateUserDto updateDto)
        {
            if (updateDto.Name != null)
            {
                user.Name = updateDto.Name;
            }
            if (updateDto.Tag != null)
            {
                user.Tag = updateDto.Tag;
            }
            if (updateDto.Description != null)
            {
                user.Description = updateDto.Description;
            }

            await _dbContext.SaveChangesAsync();
            await _PublishUserChangedNotification(await _MapUserToDtoAsync(user));
        }

        private static bool _IsIdentiedTag(string tag)
        {
            return tag.StartsWith("id") && (tag.Length >= 3) && char.IsNumber(tag[2]);
        }

        private static int _GetIdFromIdentiedTag(string tag)
        {
            string idString = tag.Substring(2);
            int id = int.Parse(idString);
            return id;
        }

        public async Task UpdateAvatarAsync(User user, string photoPath)
        {
            user.AvatarPath = photoPath;
            await _dbContext.SaveChangesAsync();
            await _PublishUserChangedNotification(await _MapUserToDtoAsync(user));
        }

        private async Task _PublishUserChangedNotification(UserDto userDto)
        {
            await _mediator.Publish(new UserChangedNotification { UserDto = userDto });
        }
    }
}
