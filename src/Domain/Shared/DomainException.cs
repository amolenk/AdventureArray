namespace AdventureArray.Domain.Shared;

[Serializable]
public class DomainException : Exception
{
	[ExcludeFromCodeCoverage]
	public DomainException() : base()
	{
	}

	[ExcludeFromCodeCoverage]
	public DomainException(string? message) : base(message)
	{
	}

	[ExcludeFromCodeCoverage]
	public DomainException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}
