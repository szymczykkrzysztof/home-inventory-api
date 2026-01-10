using HomeInventory.Domain.Aggregates.House;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Configurations;

public class HouseConfiguration : IEntityTypeConfiguration<House>
{
    public void Configure(EntityTypeBuilder<House> builder)
    {
        builder.ToTable("Houses");

        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id)
            .ValueGeneratedNever();
        builder.Property(h => h.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder
            .HasMany(h => h.Locations)
            .WithOne()
            .HasForeignKey("HouseId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.Metadata.FindNavigation(nameof(House.Locations))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}