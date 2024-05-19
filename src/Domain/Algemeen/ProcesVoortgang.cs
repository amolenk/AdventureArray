namespace AdventureArray.Domain.Algemeen;

public class ProcesVoortgang : ValueObject
{
	private ProcesVoortgang(int waarde)
	{
		Waarde = waarde;
	}

	public int Waarde { get; }

	public static ProcesVoortgang Parse(int waarde)
	{
		if (!TryParse(waarde, out var ean))
		{
			throw new DomainException($"Ongeldige proces voortgang: {waarde}");
		}

		return ean;
	}

	public static bool TryParse(int waarde, [MaybeNullWhen(false)] out ProcesVoortgang voortgang)
	{
		if (waarde is < 0 or > 100)
		{
			voortgang = null;
			return false;
		}

		voortgang = new ProcesVoortgang(waarde);
		return true;
	}

	public static implicit operator int(ProcesVoortgang voortgang)
	{
		return voortgang.Waarde;
	}

	protected override IEnumerable<object> GetAtomicValues()
	{
		yield return Waarde;
	}
}
