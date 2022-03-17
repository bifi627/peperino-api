using FluentValidation;
using Peperino_Api.Helpers;
using Peperino_Api.Libs;
using Peperino_Api.Models;
using Peperino_Api.Models.User;
using Peperino_Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFirebase();

// Add mongodb config
var mongoSettingsJson = Environment.GetEnvironmentVariable("MONGO_SETTINGS_JSON");
if (mongoSettingsJson is not null)
{
    var mongoSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<MongoSettings>(mongoSettingsJson);
    builder.Services.Configure<MongoSettings>(settings =>
    {
        settings = mongoSettings;
    });
}
else
{
    builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoSettings"));
}

// Add cors
builder.Services.AddCors(options => options.AddPolicy("DEBUG", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddCors(options => options.AddPolicy("PROD", policy =>
{
    var allowedOrigins = new List<string>();
    allowedOrigins.Add("https://peperino.vercel.app/");
    allowedOrigins.Add("https://peperino-bifi627.vercel.app/");

    var allowed = Environment.GetEnvironmentVariable("ORIGINS_JSON");
    if (allowed is not null)
    {
        allowedOrigins.AddRange(Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(allowed));
    }

    policy.WithOrigins(allowedOrigins.ToArray());
    policy.WithHeaders("Authorization");
}));

// Add services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IListService, ListService>();

// Add validator
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>(ServiceLifetime.Scoped);

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

app.MapControllers();

app.UseMiddleware<JwtMiddleware>();

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";

var url = $"http://*:{port}";

app.Run(url);

