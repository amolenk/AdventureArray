using AdventureArray.Infrastructure.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/ping", () => $"Hello from {Environment.GetEnvironmentVariable("ASPNETCORE_URLS")}!")
	.WithName("Ping")
	.WithOpenApi();

app.MapDefaultEndpoints();

app.Run();
