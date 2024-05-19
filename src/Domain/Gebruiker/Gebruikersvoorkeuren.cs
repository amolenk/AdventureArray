namespace AdventureArray.Domain.Gebruiker;

public sealed class Gebruikersvoorkeuren
{
	public string GebruikersId { get; }
	public DarkModeSetting DarkMode { get; private set; } = DarkModeSetting.Auto;

	public Gebruikersvoorkeuren(string gebruikersId)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(gebruikersId);

		GebruikersId = gebruikersId;
	}

	public void SetDarkMode(DarkModeSetting darkMode)
	{
		if (darkMode != DarkMode)
		{
			DarkMode = darkMode;
		}
	}
}

public enum DarkModeSetting
{
	Auto,
	Light,
	Dark
}
