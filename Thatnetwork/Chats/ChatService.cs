using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Thatnetwork.Chats.Dtos;
using Thatnetwork.Chats.Events;
using Thatnetwork.Entities;
using Thatnetwork.Photos;
using Thatnetwork.Users;

namespace Thatnetwork.Chats
{
    public class ChatService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public ChatService(AppDbContext dbContext, IMapper mapper, IMediator mediator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task AddMessageAsync(AddMessageDto addMessageDto, User? sender)
        {
            ChatRoom chatRoom = _dbContext.ChatRooms.Single(c => c.Id == addMessageDto.ChatRoomId);
            List<Photo> photos = new();
            foreach (int photoId in addMessageDto.PhotoIds)
            {
                Photo photo = _dbContext.Photos.Single(p => p.Id == photoId);
                photos.Add(photo);
            }

            Message message = new Message {
                ChatRoom = chatRoom,
                Text = addMessageDto.Text,
                CreationDate = DateTime.UtcNow,
                Photos = photos,
                Sender = sender,
            };

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();
            await _mediator.Publish(new MessageSentNotification { 
                ChatRoomId = chatRoom.Id,
                MessageDto = MapMessageToDto(message),
            });
        }

        public async Task EditMessageAsync(EditMessageDto dto)
        {
            Message message = await _dbContext.Messages.SingleAsync(m => m.Id == dto.MessageId);
            message.Text = dto.Text;
            List<Photo> photos = await _dbContext.Photos.Where(p => dto.PhotoIds.Contains(p.Id)).ToListAsync();
            message.Photos = photos;
            message.IsEdited = true;
            await _dbContext.SaveChangesAsync();

            await _mediator.Publish(new MessageEditedNotification
            {
                ChatRoomId = message.ChatRoomId,
                MessageDto = MapMessageToDto(message)
            });
        }

        public async Task<MessagePageDto> GetMessagePageAsync(GetMessagesDto dto)
        {
            List<Message> messages = await _dbContext.Messages
                .Where(m => m.ChatRoomId == dto.ChatRoomId)
                .Where(m => dto.start == null || (m.CreationDate < dto.start))
                .OrderByDescending(m => m.CreationDate)
                .Take(dto.Count + 1)
                .ToListAsync();

            DateTime? next = null;
            if (messages.Count == dto.Count+1)
            {
                next = messages.Last().CreationDate;
            }

            return new MessagePageDto {
                Items = MapMessagesToDtos(messages),
                Next = next,
            };
        }

        //public async Task<ChatRoomPageDto> GetChatRoomPageAsync(GetChatRoomsDto dto, int userId)
        //{
        //    List<ChatRoom> chatRooms;

        //    var query = _dbContext.ChatRooms
        //        .Include(c => c.Participants)
        //        .Where(c => c.Participants.Any(p => p.Id == userId))
        //        .Include(c => c.Messages.OrderByDescending(m => m.CreationDate).SingleOrDefault())
        //        .OrderBy(c => c.Messages.FirstOrDefault() == null);
            
        //    if (dto.start == null)
        //    {
        //        chatRooms = await query.Take(dto.Count).ToListAsync();
        //    } else
        //    {
        //        chatRooms = await query
        //            .Where(c => c.Messages.FirstOrDefault().Equals(null) || c.Messages.FirstOrDefault()!.CreationDate < dto.start)
        //            .Take(dto.Count)
        //            .ToListAsync();
        //    }

        //    var takeQuery = query.Take(20);

        //}

        //public async Task<Message?> getLatestMessage
        //{

        //}

        public async Task<List<ChatRoomDto>> GetChatRoomsAsync(int userId)
        {
            List<ChatRoom> chatRooms = await _dbContext.ChatRooms
                .Include(c => c.Participants)
                .Where(c => c.Participants.Any(p => p.Id == userId))
                .ToListAsync();
            return MapChatRoomsToDtos(chatRooms, userId);
        }

        public async Task<ChatRoomDto> GetChatRoomByIdAsync(int chatRoomId, int userId)
        {
            ChatRoom chatRoom = await _dbContext.ChatRooms.SingleAsync(c => c.Id == chatRoomId);
            return MapChatRoomToDto(chatRoom, userId);
        }

        public List<ChatRoomDto> MapChatRoomsToDtos(List<ChatRoom> chatRooms, int userId)
        {
            return chatRooms.Select(c => MapChatRoomToDto(c, userId)).ToList();
        }

        public ChatRoomDto MapChatRoomToDto(ChatRoom chatRoom, int userId)
        {
            MessageDto? latestMessage = GetLatestMessageOrDefault(chatRoom.Id);
            //int participantsCount = _dbContext.Users
            //    .Include(u => u.ChatRooms.Where(c => c.Id == chatRoom.Id))
            //    .Count();

            DateTime? lastReadDate = _dbContext.UserChatRooms
                .SingleOrDefault(ucr => ucr.ChatRoomId == chatRoom.Id &&  ucr.UserId == userId)
                ?.LastReadDate;
            int unreadMessagesCount;
            var roomMessagesQuery = _dbContext.Messages
                .Where(m => m.ChatRoomId == chatRoom.Id);
            if (lastReadDate == null)
            {
                unreadMessagesCount = roomMessagesQuery.Count();
            } else
            {
                unreadMessagesCount = roomMessagesQuery.Where(m => m.CreationDate > lastReadDate).Count();
            }

            return new ChatRoomDto {
                Id = chatRoom.Id,
                Name = chatRoom.Name ?? "Тут должно быть имя пользователя, а не название беседы",
                LatestMessage = latestMessage,
                UnreadMessagesCount = unreadMessagesCount,
                //ParticipantsCount = participantsCount,
            };
        }

        

        public MessageDto MapMessageToDto(Message message)
        {
            return _mapper.Map<MessageDto>(message);
        }

        public List<MessageDto> MapMessagesToDtos(List<Message> messages)
        {
            return messages.Select(m => MapMessageToDto(m)).ToList();
        }

        public MessageDto? GetLatestMessageOrDefault(int chatRoomId)
        {
            Message? message = _dbContext.Messages
                .Where(m => m.ChatRoomId == chatRoomId)
                .OrderByDescending(m => m.CreationDate)
                .FirstOrDefault();
            if (message == null)
            {
                return null;
            }
            return MapMessageToDto(message);
        }
    }
}
