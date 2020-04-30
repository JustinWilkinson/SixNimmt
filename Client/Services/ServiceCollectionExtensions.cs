using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SixNimmt.Client.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameStorage(this IServiceCollection services)
        {
            services.AddStorage();
            services.TryAddScoped<GameStorage>();
        }
    }
}