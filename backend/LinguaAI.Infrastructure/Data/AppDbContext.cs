using LinguaAI.Infrastructure.Entities;
using LinguaAI.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LinguaAI.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<TranslationHistory> TranslationHistories => Set<TranslationHistory>();
    public DbSet<DictionaryEntry> DictionaryEntries => Set<DictionaryEntry>();
    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TranslationHistory>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.SourceText).HasMaxLength(8000).IsRequired();
            entity.Property(x => x.TranslatedText).HasMaxLength(8000).IsRequired();
            entity.Property(x => x.SourceLanguage).HasMaxLength(16).IsRequired();
            entity.Property(x => x.TargetLanguage).HasMaxLength(16).IsRequired();
            entity.Property(x => x.Style).HasMaxLength(32).IsRequired();
            entity.Property(x => x.OperationType).HasMaxLength(32).IsRequired();
            entity.HasIndex(x => new { x.UserId, x.CreatedAtUtc });
            entity.HasOne(x => x.User)
                .WithMany(x => x.TranslationHistories)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<DictionaryEntry>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Term).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Translation).HasMaxLength(256).IsRequired();
            entity.Property(x => x.SourceLanguage).HasMaxLength(16).IsRequired();
            entity.Property(x => x.TargetLanguage).HasMaxLength(16).IsRequired();
            entity.Property(x => x.ExampleSentence).HasMaxLength(1000);
            entity.Property(x => x.Notes).HasMaxLength(1000);
            entity.HasIndex(x => new { x.UserId, x.Term });
            entity.HasOne(x => x.User)
                .WithMany(x => x.DictionaryEntries)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<UserPreference>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.PreferredSourceLanguage).HasMaxLength(16);
            entity.Property(x => x.PreferredTargetLanguage).HasMaxLength(16);
            entity.Property(x => x.PreferredStyle).HasMaxLength(32);
            entity.HasIndex(x => x.UserId).IsUnique();
            entity.HasOne(x => x.User)
                .WithOne(x => x.Preference)
                .HasForeignKey<UserPreference>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
