using AdventureArray.Domain.Rides;

namespace AdventureArray.Infrastructure.Persistence.EntityTypeConfigurations;

public class RideWaitTimeConfiguration : IEntityTypeConfiguration<RideWaitTime>
{
	public void Configure(EntityTypeBuilder<RideWaitTime> builder)
	{
		builder.HasKey(e => e.Id);
	}
}
