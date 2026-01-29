using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TemplateJwtProject.Models;

namespace TemplateJwtProject.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Top2000Entry> Top2000Entry { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<PlayList> PlayLists { get; set; }
    public DbSet<Songs> Songs { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<PlayListSong> PlayListSongs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.PlayLists)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RefreshToken>()
            .HasIndex(rt => rt.Token)
            .IsUnique();

        builder.Entity<Top2000Entry>()
            .HasKey(e => new { e.SongId, e.Year });

        builder.Entity<Songs>()
            .HasOne(s => s.Artist)
            .WithMany()
            .HasForeignKey(s => s.ArtistId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PlayListSong>()
            .HasKey(ps => new { ps.PlayListId, ps.SongId });

        builder.Entity<PlayListSong>()
            .HasOne(ps => ps.PlayList)
            .WithMany(p => p.PlayListSongs)
            .HasForeignKey(ps => ps.PlayListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PlayListSong>()
            .HasOne(ps => ps.Song)
            .WithMany(s => s.PlayListSongs)
            .HasForeignKey(ps => ps.SongId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}