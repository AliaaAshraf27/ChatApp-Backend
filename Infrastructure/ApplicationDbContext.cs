using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMember> ChatMembers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserBlock> UserBlocks { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<StatusView> StatusViews { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserBlock>(entity =>
            {
                entity.HasKey(x => x.Id);

                // User who blocked
                entity.HasOne(x => x.User)
                    .WithMany(x => x.BlockedByMe)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // User who was blocked
                entity.HasOne(x => x.BlockedUser)
                    .WithMany(x => x.BlockedMe)
                    .HasForeignKey(x => x.BlockedUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserBlock>()
                .HasIndex(b => new { b.UserId, b.BlockedUserId })
                .IsUnique();


            modelBuilder.Entity<Status>()
              .HasOne(s => s.User)
              .WithMany()
              .HasForeignKey(s => s.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StatusView>()
                .HasOne(v => v.Status)
                .WithMany(s => s.Views)
                .HasForeignKey(v => v.StatusId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StatusView>()
                .HasOne(v => v.Viewer)
                .WithMany()
                .HasForeignKey(v => v.ViewerId)
                .OnDelete(DeleteBehavior.Restrict);

            // كل مستخدم يشوف الحالة مرة واحدة بس تتسجل (مش كل مرة يفتحها)
            modelBuilder.Entity<StatusView>()
                .HasIndex(v => new { v.StatusId, v.ViewerId })
                .IsUnique();

            // Index مهم جدًا للأداء - هنستعلم عليه كتير
            modelBuilder.Entity<Status>()
                .HasIndex(s => s.ExpiresAt);

            modelBuilder.Entity<Status>()
                .HasIndex(s => s.UserId);

            modelBuilder.Entity<Contact>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contact>()
                .HasOne(c => c.ContactUser)
                .WithMany()
                .HasForeignKey(c => c.ContactUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Contact>()
                .HasIndex(c => new { c.UserId, c.ContactUserId })
                .IsUnique();

            // جلب رسايل chat مرتبة بالوقت
            modelBuilder.Entity<Message>()
                .HasIndex(m => new { m.ChatId, m.SentAt });

            // GetChatIdsAsync و IsMemberAsync
            modelBuilder.Entity<ChatMember>()
                .HasIndex(cm => new { cm.UserId, cm.ChatId });

            // منع تكرار العضو في نفس الشات
            modelBuilder.Entity<ChatMember>()
                .HasIndex(cm => new { cm.ChatId, cm.UserId })
                .IsUnique();
        }
    }
}
