using AdventureArray.Application.UI.Client.Infrastructure.Berichten;
using AdventureArray.Application.UI.Client.Infrastructure.ErrorHandling;
using AdventureArray.Application.UI.Client.Infrastructure.Http;
using AdventureArray.Application.UI.Client.Infrastructure.Identity;
using AdventureArray.Application.UI.Client.Infrastructure.Messaging;
using AdventureArray.Application.UI.Model;
using FluentValidation;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


builder.AddCustomMassTransitMediator();

// Register a logger to process unhandled exceptions.
builder.Services.AddLogging(loggingBuilder =>
{
	loggingBuilder.Services.TryAddEnumerable(
		ServiceDescriptor.Singleton<ILoggerProvider, UnhandledExceptionLoggerProvider>());
});

builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddMudServices();
builder.Services.AddMapster();
builder.Services.AddSingleton<IBerichtenManager, BerichtenManager>();
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddValidatorsFromAssembly(typeof(ModelAssemblyLocator).Assembly);
builder.Services.AddHotKeys2();

// Register the HttpClient with a custom handler that throws an ApiRequestException if
// the request is not successful.
builder.Services.AddScoped(sp =>
{
	var ensureSuccessHandler = new ApiRequestHandler()
	{
		InnerHandler = new HttpClientHandler()
	};

	return new HttpClient(ensureSuccessHandler)
	{
		BaseAddress = new Uri(builder.HostEnvironment.BaseAddress + "api/")
	};
});

// Register all API services.
builder.Services.Scan(scan => scan
	.FromAssemblyOf<Program>()
	.AddClasses(classes => classes.AssignableTo<IApiService>())
	.AsSelfWithInterfaces()
	.WithScopedLifetime());

await builder.Build().RunAsync();
