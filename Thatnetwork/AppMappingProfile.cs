using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using Thatnetwork.Challenges;
using Thatnetwork.Challenges.Dtos;
using Thatnetwork.Chats;
using Thatnetwork.Chats.Dtos;
using Thatnetwork.Clubs;
using Thatnetwork.Clubs.Dtos;
using Thatnetwork.Entities;
using Thatnetwork.Notes;
using Thatnetwork.Notes.Dtos;
using Thatnetwork.Photos;
using Thatnetwork.Photos.Dtos;
using Thatnetwork.Users;
using Thatnetwork.Users.Dtos;

namespace Thatnetwork
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(u => u.Tag, opt => opt.MapFrom(uEntity => uEntity.Tag ?? $"id{uEntity.Id}"));
            CreateMap<Note, NoteDto>();
            CreateMap<Club, ClubDto>();
            CreateMap<Photo, PhotoDto>();
            CreateMap<Marathon, MarathonDto>()
                .ForMember(m => m.Hashtags, opt => opt.MapFrom(mEntity => mEntity.Hashtags.Select(h => h.Text)));
            CreateMap<ChatRoom, ChatRoomDto>();
            CreateMap<Message, MessageDto>();
        }
    }
}
