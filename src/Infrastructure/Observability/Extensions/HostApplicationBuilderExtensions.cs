using Azure.Monitor.OpenTelemetry.AspNetCore;
using AdventureArray.Infrastructure.Features;
using Microsoft.AspNetCore.Http.Features;

namespace AdventureArray.Infrastructure.Observability.Extensions;

/// <summary>
/// Extension methods for <see cref="IHostApplicationBuilder"/>.
/// </summary>
public static class HostApplicationBuilderExtensions
{
	public static void AddCustomExceptionHandling(this IHostApplicationBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(builder);

		builder.Services.AddProblemDetails(options =>
			options.CustomizeProblemDetails = (context) =>
			{
				if (context.ProblemDetails.Extensions.ContainsKey("traceId")) return;

				// Add traceId to problem details.
				var activityFeature = context.HttpContext.Features.Get<IHttpActivityFeature>();
				var activity = activityFeature?.Activity;
				if (activity != null)
				{
					context.ProblemDetails.Extensions.Add(new KeyValuePair<string, object?>("traceId", activity.TraceId.ToString()));
				}
			}
		);
	}

	public static void AddCustomOpenTelemetry(this IHostApplicationBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(builder);

		var settings = GetSettings(builder);

		void ConfigureResource(ResourceBuilder r)
		{
			r.AddService(
				builder.Environment.ApplicationName,
				serviceVersion: Assembly.GetCallingAssembly().GetName().Version?.ToString() ?? "unknown",
				serviceInstanceId: Environment.MachineName);
		}

		var otelBuilder = builder.Services.AddOpenTelemetry()
			.ConfigureResource(ConfigureResource);

		if (settings.AzureMonitor is not null)
		{
			otelBuilder.UseAzureMonitor(options =>
			{
				options.ConnectionString = settings.AzureMonitor.ApplicationInsightsConnectionString;
			});
		}

		if (!builder.Properties.TryGetValue(nameof(FeatureRegistry), out var property))
		{
			throw new InvalidOperationException(
				"FeatureRegistry is not registered. Make sure AddCustomFeatures is called before AddCustomOpenTelemetry.");
		}
		var featureRegistry = (FeatureRegistry)property;

		var meterNames = featureRegistry.GetInstrumentationMeterNames();

		ConfigureOpenTelemetryLogging(builder, settings.Logging, builder.Environment.ApplicationName);
		ConfigureOpenTelemetryMetrics(otelBuilder, meterNames, settings.Metrics);
		ConfigureOpenTelemetryTracing(otelBuilder, settings.Tracing);
	}

	private static void ConfigureOpenTelemetryLogging(IHostApplicationBuilder builder, OpenTelemetryLoggingSettings settings, string applicationName)
	{
		// If we want console logging, just keep the existing provider. The OTEL console exporter does not
		// output a human-friendly format, so we need to keep the existing provider.
		if (!settings.ExportToConsole)
		{
			builder.Logging.ClearProviders();
		}

		if (settings.OpenTelemetryProtocol is not null)
		{
			builder.Logging.AddOpenTelemetry(options =>
			{
				options.IncludeFormattedMessage = true;
				options.IncludeScopes = true;

				options.AddOtlpExporter(o =>
					{
						o.Protocol = OtlpExportProtocol.HttpProtobuf;
						o.Endpoint = new Uri(settings.OpenTelemetryProtocol.Endpoint ?? string.Empty);
					});
			});
		}
	}

	private static void ConfigureOpenTelemetryMetrics(IOpenTelemetryBuilder otelBuilder, IEnumerable<string> meterNames,
		OpenTelemetryMetricsSettings settings)
	{
		otelBuilder.WithMetrics(b =>
		{
			var meterProviderBuilder = b
				.AddAspNetCoreInstrumentation()
				.AddMeter(InstrumentationOptions.MeterName);// MassTransit Meter

			foreach (var meter in meterNames)
			{
				meterProviderBuilder.AddMeter(meter);
			}

			if (settings.ExportToConsole)
			{
				meterProviderBuilder.AddConsoleExporter();
			}

			if (settings.Prometheus?.ExposeScrapingEndpoint ?? false)
			{
				meterProviderBuilder.AddPrometheusExporter();
			}
		});
	}

	private static void ConfigureOpenTelemetryTracing(IOpenTelemetryBuilder otelBuilder, OpenTelemetryTracingSettings settings)
	{
		otelBuilder.WithTracing(b =>
		{
			var traceProviderBuilder = b
				.AddAspNetCoreInstrumentation(configure =>
				{
					configure.RecordException = true;
				})
				.AddEntityFrameworkCoreInstrumentation(configure =>
				{
					configure.SetDbStatementForText = true;

					// Exclude outbox state and transaction commands from EF Core telemetry
					configure.Filter = (_, command) => !command.CommandText.Contains("FROM outbox_message")
						&& !command.CommandText.Contains("FROM \"outbox_state\"")
						&& !command.CommandText.Contains("FROM inbox_state")
						&& !command.CommandText.Contains("UPDATE outbox_state");
				})
				.AddHttpClientInstrumentation()
				.AddSource(DiagnosticHeaders.DefaultListenerName) // MassTransit ActivitySource
				.AddSource(ApplicationActivitySource.Name); // Custom tracing

			if (settings.ExportToConsole)
			{
				traceProviderBuilder.AddConsoleExporter();
			}

			if (settings.OpenTelemetryProtocol is not null)
			{
				traceProviderBuilder.AddOtlpExporter(o =>
				{
					o.Endpoint = new Uri(settings.OpenTelemetryProtocol.Endpoint ?? string.Empty);
				});
			}
		});
	}

	private static OpenTelemetrySettings GetSettings(IHostApplicationBuilder builder)
	{
		OpenTelemetrySettings settings = new();
		builder.Configuration.GetSection(OpenTelemetrySettings.ConfigurationSectionName).Bind(settings);
		return settings;
	}
}
