using AdventureArray.Domain.Algemeen;

namespace AdventureArray.Infrastructure.Persistence.EntityTypeConfigurations;

public class ProcesConfiguration : IEntityTypeConfiguration<Proces>
{
	public void Configure(EntityTypeBuilder<Proces> builder)
	{
		builder.HasKey(e => e.Id);

		builder.Property(e => e.Voortgang)
			.HasConversion(
				x => x!.Waarde,
				x => ProcesVoortgang.Parse(x));

		builder.Property(e => e.Tijdstip)
			.HasColumnType("timestamp with time zone")
			.HasConversion(
				v => v.UtcDateTime,
				v => new DateTimeOffset(v));

		builder.Property(e => e.Versie)
			.IsConcurrencyToken();
	}
}
