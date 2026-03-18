using Microsoft.EntityFrameworkCore;
using MyAPI.Models.Entities;
using System.Security;

namespace MyAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<UserDevice> UserDevices { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<Conversation> Conversations => Set<Conversation>();

        public DbSet<ConversationMember> ConversationMembers => Set<ConversationMember>();

        public DbSet<Message> Messages => Set<Message>();

        public DbSet<MessageRead> MessageReads => Set<MessageRead>();

        public DbSet<Attachment> Attachments => Set<Attachment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserDevice>()
                .HasIndex(x => new { x.UserId, x.RefreshToken })
                .IsUnique();

            // UserRoles
            modelBuilder.Entity<UserRole>()
                .HasKey(x => new { x.UserId, x.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId);

            // RolePermissions
            modelBuilder.Entity<RolePermission>()
                .HasKey(x => new { x.RoleId, x.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(x => x.Role)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(x => x.Permission)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.PermissionId);

            // UserProfile
            modelBuilder.Entity<UserProfile>()
                .HasOne(p => p.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Chat entities
            modelBuilder.Entity<Message>()
                .HasIndex(x => new { x.ConversationId, x.CreatedAt });

            modelBuilder.Entity<ConversationMember>()
                .HasIndex(x => new { x.UserId, x.ConversationId });

            modelBuilder.Entity<MessageRead>()
                .HasIndex(x => x.MessageId);

            modelBuilder.Entity<Attachment>()
                .HasIndex(x => x.MessageId);

            modelBuilder.Entity<Conversation>()
                .HasIndex(x => x.CreatedAt);

            base.OnModelCreating(modelBuilder);
        }
    }
}