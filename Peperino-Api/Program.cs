using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Peperino_Api.Helpers;
using Peperino_Api.Hubs;
using Peperino_Api.Models.User;
using Peperino_Api.Startup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFirebase();
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddCustomCors();
builder.Services.AddPeperinoServices();

// Add validator
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>(ServiceLifetime.Scoped);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddSignalR();

// Initialize the default app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors("DEBUG");
}
else
{
    app.UseCors("PROD");
}

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.UseMiddleware<AuthMiddleware>();

app.UseWebSockets();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<ListHub>("/signalr/list");

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";

var url = $"http://*:{port}";

app.Run(url);

