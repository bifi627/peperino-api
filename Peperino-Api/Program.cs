using Peperino_Api.Helpers;
using Peperino_Api.Libs;
using Peperino_Api.Models;
using Peperino_Api.Services;
using FluentValidation;
using Peperino_Api.Models.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFirebase();

builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoSettings"));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IListService, ListService>();

builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>(ServiceLifetime.Scoped);

// Initialize the default app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<JwtMiddleware>();

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";

var url = $"http://*:{port}";

app.Run(url);
