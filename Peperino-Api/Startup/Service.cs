using Peperino_Api.Services;

namespace Peperino_Api.Startup
{
    public static class Service
    {
        public static IServiceCollection AddPeperinoServices(this IServiceCollection services)
        {
            Console.WriteLine($"[Service] Adding service {nameof(UserManagementService)}");
            services.AddScoped<IUserManagementService, UserManagementService>();

            Console.WriteLine($"[Service] Adding service {nameof(ListService)}");
            services.AddScoped<IListService, ListService>();

            Console.WriteLine($"[Service] Adding service {nameof(NotificationService)}");
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}
