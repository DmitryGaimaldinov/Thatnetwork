using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Thatnetwork.Auth;
using Thatnetwork.Challenges;
using Thatnetwork.Chats;
using Thatnetwork.Clubs;
using Thatnetwork.Notes;
using Thatnetwork.Photos;
using Thatnetwork.Users;

namespace Thatnetwork.Entities
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Note> Notes { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<Club> Clubs { get; set; } = null!;
        public DbSet<Photo> Photos {  get; set; } = null!;
        public DbSet<Marathon> Marathons { get; set; } = null!;
        public DbSet<ChallengeHashtag> ChallengeHashtags { get; set; } = null!;
        public DbSet<ChatRoom> ChatRooms { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<UserChatRoom> UserChatRooms {  get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            //Database.EnsureCreated();
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Tag)
                .IsUnique();
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasMany(u => u.Notes)
                    .WithOne(n => n.Creator)
                    .HasForeignKey(n => n.CreatorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.CreatedClubs)
                    .WithOne(c => c.Creator)
                    .HasForeignKey(c => c.CreatorId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(u => u.CreatedMaraphons)
                    .WithOne(c => c.Creator)
                    .HasForeignKey(c => c.CreatorId)
                    .OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(u => u.TakenMaraphones)
                    .WithMany(c => c.Participants);

                entity.HasMany(u => u.CreateChatRooms)
                    .WithOne(c => c.Creator)
                    .OnDelete(DeleteBehavior.NoAction);
                entity.HasMany(u => u.ChatRooms)
                    .WithMany(cr => cr.Participants);
            });

            // Note
            modelBuilder.Entity<Note>()
                .Navigation(n => n.Creator)
                .AutoInclude();
            modelBuilder.Entity<Note>()
                .Navigation(n => n.Photos)
                .AutoInclude();
            modelBuilder.Entity<Note>()
                .HasMany(n => n.Children)
                .WithOne(n => n.Parent)
                .OnDelete(DeleteBehavior.NoAction);

            // Club
            modelBuilder.Entity<Club>()
                .HasIndex(c => c.Tag)
                .IsUnique();
            modelBuilder.Entity<Club>()
                .Navigation(c => c.Avatar)
                .AutoInclude();


            // Marathon
            modelBuilder.Entity<Marathon>()
                .HasIndex(u => u.Tag)
                .IsUnique();
            modelBuilder.Entity<Marathon>()
                .Navigation(n => n.Creator)
                .AutoInclude();
            modelBuilder.Entity<Marathon>()
                .Navigation(n => n.Hashtags)
                .AutoInclude();
            modelBuilder.Entity<Marathon>()
                .Navigation(n => n.Avatar)
                .AutoInclude();
            modelBuilder.Entity<Marathon>()
                .HasOne(m => m.ChatRoom)
                .WithOne(cr => cr.Marathon)
                .HasForeignKey<Marathon>(m => m.ChatRoomId);

            // Message
            modelBuilder.Entity<Message>()
                .Navigation(m => m.Sender)
                .AutoInclude();
            modelBuilder.Entity<Message>()
                .Navigation(m => m.Photos)
                .AutoInclude();
        }
    }
}
