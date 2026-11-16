
using Microsoft.EntityFrameworkCore;
using TrackManagement.Domain.Entities;

namespace TrackManagement.Infrastructure.Data;

public class TrackManagementDbContext : DbContext
{
    public TrackManagementDbContext(
        DbContextOptions<TrackManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<Track> Tracks => Set<Track>();
    public DbSet<DSP> DSPs => Set<DSP>();
    public DbSet<TrackDistribution> TrackDistributions
        => Set<TrackDistribution>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ARTIST
        modelBuilder.Entity<Artist>(entity =>
        {
            //primary key
            entity.HasKey(a => a.Id);

            // cannot be null and max length 200 characters
            entity.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(a => a.Email)
                .IsRequired()
                .HasMaxLength(320);

            entity.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(100);
        });

        // TRACK
        modelBuilder.Entity<Track>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(t => t.Isrc)
                .IsRequired()
                .HasMaxLength(12);

            //ensure that the ISRC is unique and no 2 tracks can have the same ISRC
            entity.HasIndex(t => t.Isrc)
                .IsUnique();

            entity.Property(t => t.Genre)
                .IsRequired()
                .HasMaxLength(100);

            //has conversion to string makes it readable
            entity.Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.HasOne(t => t.Artist)
                .WithMany(a => a.Tracks)
                .HasForeignKey(t => t.ArtistId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // DSP
        modelBuilder.Entity<DSP>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(d => d.Name)
                .IsUnique();
        });

        // TRACK DISTRIBUTION
        modelBuilder.Entity<TrackDistribution>(entity =>
        {
            entity.HasKey(td => td.Id);

            entity.Property(td => td.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            //composite unique index
            // prevents accidentally creating two distribution records for the same track on the same platform
            entity.HasIndex(td => new { td.TrackId, td.DspId })
                .IsUnique();

            entity.HasOne(td => td.Track)
                .WithMany(t => t.TrackDistributions)
                .HasForeignKey(td => td.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(td => td.Dsp)
                .WithMany(d => d.TrackDistributions)
                .HasForeignKey(td => td.DspId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
