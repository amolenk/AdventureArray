namespace AdventureArray.Application.UI.Client.Infrastructure.ErrorHandling;

/// <summary>
/// Provides the settings for the error dialog.
/// </summary>
public sealed class ErrorDialogSettings
{
	public const string ConfigurationSectionName = "ErrorDialog";

	/// <summary>
	/// The e-mail address to send problem reports to.
	/// </summary>
	public string? ProblemReportRecipient { get; set; }

	/// <summary>
	/// The URL format for navigating to the exception trace.
	/// </summary>
	public string? TraceUrlFormat { get; set; }
}
