using Peperino_Api.Services;

namespace Peperino_Api.Startup
{
    public static class Service
    {
        public static IServiceCollection AddPeperinoServices(this IServiceCollection services)
        {
            Console.WriteLine($"[Service] Adding service {nameof(UserService)}");
            services.AddScoped<IUserService, UserService>();

            Console.WriteLine($"[Service] Adding service {nameof(ListService)}");
            services.AddScoped<IListService, ListService>();
            return services;
        }
    }
}
