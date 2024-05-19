using AdventureArray.Domain.Gebruiker;

namespace AdventureArray.Infrastructure.Persistence.EntityTypeConfigurations;

public class GebruikersvoorkeurenConfiguration : IEntityTypeConfiguration<Gebruikersvoorkeuren>
{
	public void Configure(EntityTypeBuilder<Gebruikersvoorkeuren> builder)
	{
		builder.HasKey(e => e.GebruikersId);

		// TODO Add concurrency token
		// builder.Property(e => e.Version)
		//     .IsConcurrencyToken();
	}
}
