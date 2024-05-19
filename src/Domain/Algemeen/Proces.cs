using System.Text.Json;

namespace AdventureArray.Domain.Algemeen;

public class Proces : IConcurrentEntity
{
	public Guid Id { get; init; }
	public string Naam { get; init; } = string.Empty;
	public ProcesVoortgang? Voortgang { get; set; }
	public ProcesStatus Status { get; set; }
	public DateTimeOffset Tijdstip { get; set; } = DateTimeOffset.UtcNow;
	public string? ExtraInformatie { get; set; }
	public string? Data { get; private set; }
	public long Versie { get; set; }

	public TData? LeesData<TData>()
	{
		return Data is null ? default : JsonSerializer.Deserialize<TData>(Data);
	}

	public void SchrijfData<TData>(TData data)
	{
		Data = JsonSerializer.Serialize(data);
	}
}
