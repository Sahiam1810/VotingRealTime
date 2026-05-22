using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Extensions;

// se configuro por buena practica no es estrictamente necesario porque el HTML esta en el mismo servidor
// se usa para ´proyectos donde el fronted esta en un origen diferente
public static class CorsServiceExtensions
{
    
    public static void ConfigureCors(this IServiceCollection services) =>
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader());
        });
}