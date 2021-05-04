using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace WerkWerk
{
    using Data;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWerk<TContext>(this IServiceCollection services) where TContext : DbContext
        {
            services.AddScoped<IWorkRepository, WorkRepository<TContext>>();
            services.AddLogging();

            return services;
        }
    }
}