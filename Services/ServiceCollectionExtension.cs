using Microsoft.Extensions.DependencyInjection;
using Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddShapeUpService(this IServiceCollection services)
        {
            services.AddScoped<IShapeUpService, ShapeUpService>();
            services.AddScoped<IShapeUpRepository, ShapeUpRepository>();
            services.AddRepositories();
            return services;
        }
    }
}
