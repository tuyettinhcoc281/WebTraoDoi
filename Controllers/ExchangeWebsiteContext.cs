using ExchangeWebsite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExchangeWebsite.Controllers;

public class ExchangeWebsiteContext : IdentityDbContext<IdentityUser>
{
    public ExchangeWebsiteContext(DbContextOptions<ExchangeWebsiteContext> options)
        : base(options)
    {
    }
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<PostImage> PostImages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure ASP.NET Identity customizations
        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());

        // Configure the self-referencing relationship for Categories
        builder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Post-Category relationship
        builder.Entity<Post>()
            .HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure Post-PostImage relationship
        builder.Entity<PostImage>()
            .HasOne(pi => pi.Post)
            .WithMany(p => p.PostImages)
            .HasForeignKey(pi => pi.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);
    }
}