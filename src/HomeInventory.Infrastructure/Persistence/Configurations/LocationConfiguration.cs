using HomeInventory.Domain.Aggregates.House;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedNever();
        builder.OwnsOne(l => l.Room, room =>
        {
            room.Property(r => r.Name)
                .HasColumnName("RoomName")
                .IsRequired()
                .HasMaxLength(100);
        });

        builder.OwnsOne(l => l.Container, container =>
        {
            container.Property(c => c.Name)
                .HasColumnName("ContainerName")
                .HasMaxLength(100);
        });

        builder
            .HasMany(l => l.Items)
            .WithOne()
            .HasForeignKey("LocationId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.Metadata.FindNavigation(nameof(Location.Items))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}