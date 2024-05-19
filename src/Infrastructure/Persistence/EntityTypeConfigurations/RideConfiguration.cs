using AdventureArray.Domain.Rides;

namespace AdventureArray.Infrastructure.Persistence.EntityTypeConfigurations;

public class RideConfiguration : IEntityTypeConfiguration<Ride>
{
	public void Configure(EntityTypeBuilder<Ride> builder)
	{
		builder.HasKey(e => e.Id);
	}
}
