using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    // existe por arquitectura, en producción aquí iría la base de datos, etc.
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services)
        {
            return services;
        }
    }
}