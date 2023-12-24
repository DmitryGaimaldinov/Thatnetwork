namespace Thatnetwork.Chats.Dtos
{
    public class ChatRoomDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required MessageDto? LatestMessage {  get; set; }
        //public required int ParticipantsCount {  get; set; }
        
        public required int UnreadMessagesCount { get; set; }
    }

    //public enum ChatRoomType
    //{
    //    Dialog,
    //    Conversation,
    //    Challenge
    //}
}
