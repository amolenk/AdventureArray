namespace AdventureArray.Application.UI.Client.Features.DeveloperTools.Models;

/// <summary>
/// Provides the developer tools settings.
/// </summary>
public sealed class DeveloperToolsSettings
{
	public const string ConfigurationSectionName = "DeveloperTools";

	/// <summary>
	/// A list of links to show in the developer tools menu.
	/// </summary>
	public IDictionary<string, string>? Links { get; set; }
}
