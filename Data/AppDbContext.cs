using DwitterSocial.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DwitterSocial.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, int, 
        IdentityUserClaim<int>,AppUserRole,IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        public DbSet<UserLikes> Likes { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
                .HasMany(p => p.UserRoles)
                .WithOne(p => p.User)
                .HasForeignKey(s => s.UserId)
                .IsRequired();

            modelBuilder.Entity<AppRole>()
            .HasMany(p => p.UserRoles)
            .WithOne(p => p.Role)
            .HasForeignKey(s => s.RoleId)
            .IsRequired();

            modelBuilder.Entity<UserLikes>()
                .HasKey(k => new { k.SourceUserId, k.LikedUserId });

            modelBuilder.Entity<UserLikes>()
                .HasOne(u => u.SourceUser)
                .WithMany(u => u.LikedUsers)
                .HasForeignKey(u => u.SourceUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserLikes>()
            .HasOne(u => u.LikedUser)
            .WithMany(u => u.LikedByUsers)
            .HasForeignKey(u => u.LikedUserId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne(u => u.Receiver)
                .WithMany(s => s.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(s => s.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
