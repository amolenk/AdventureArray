namespace AdventureArray.Infrastructure.Observability.Extensions;

/// <summary>
/// Extension methods for <see cref="WebApplication"/>.
/// </summary>
public static class WebApplicationExtensions
{
	public static void UseCustomExceptionHandling(this WebApplication app)
	{
		ArgumentNullException.ThrowIfNull(app);

		app.UseExceptionHandler(new ExceptionHandlerOptions()
		{
			ExceptionHandler = async (ctx) =>
			{
				// Explicitly record the exception in the current activity.
				// See https://github.com/open-telemetry/opentelemetry-dotnet/issues/4842
				var feature = ctx.Features.Get<IExceptionHandlerFeature>();
				if (feature is not null)
				{
					Activity.Current?.RecordException(feature.Error);

					await Results
						.Problem(
							title: "Er is een onverwachte fout opgetreden.",
							detail: feature.Error.Message)
						.ExecuteAsync(ctx);
				}
				else
				{
					await Results.Problem().ExecuteAsync(ctx);
				}
			}
		});

		app.UseStatusCodePages();
	}

	public static void UseCustomOpenTelemetry(this WebApplication app)
	{
		ArgumentNullException.ThrowIfNull(app);

		var settings = GetSettings(app.Configuration);

		if (settings.Metrics?.Prometheus is not null)
		{
			app.UseOpenTelemetryPrometheusScrapingEndpoint();
		}
	}

	private static OpenTelemetrySettings GetSettings(IConfiguration configuration)
	{
		OpenTelemetrySettings settings = new();
		configuration.GetSection(OpenTelemetrySettings.ConfigurationSectionName).Bind(settings);
		return settings;
	}
}
