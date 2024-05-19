using AdventureArray.Domain.Algemeen;

namespace AdventureArray.Application.UI.Model.Algemeen;

public record HaalProcessenOpResponse(List<Proces> Processen);

public record Proces(
	Guid Id,
	string Naam,
	int? Voortgang,
	ProcesStatus Status,
	DateTimeOffset Tijdstip,
	string? ExtraInformatie);
