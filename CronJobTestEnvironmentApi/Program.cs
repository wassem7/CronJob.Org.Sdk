using System.Reflection;
using System.Text;
using CronJob.Org.Sdk.Services.Interfaces;
using CronJob.Org.Sdk.Services.Providers;
using Microsoft.OpenApi.Models;

namespace CronJobTestEnvironmentApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        string corsPolicyName = "CronJob.PolicyName";
        var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");
        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddAuthorization();
   
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        builder
            .Services
            .AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme()
                    {
                        Description =
                            "JWT Authorization header using the Bearer scheme. \r\n\r\n "
                            + "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n"
                            + "Example: \"Bearer 12345abcdef\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Scheme = "Bearer"
                    }
                );
                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header
                            },
                            new List<string>()
                        }
                    }
                );

                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo()
                    {
                        Version = "v1.0",
                        Title = "Cron Job Test Api",
                        Description = "For testing sdks",
                    }
                );

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                options.IncludeXmlComments(xmlPath);
            });
        

        builder
            .Services
            .AddCors(
                options =>
                    options.AddPolicy(
                        corsPolicyName,
                        policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
                    )
            );
        
        builder.Services.AddHttpClient();

        builder.Services.AddScoped<ICronJobService, CronJobService>();
        
        var app = builder.Build();
        app.UseCors(builder =>
        {
            builder.AllowAnyOrigin();
            builder.AllowAnyMethod();
            builder.AllowAnyHeader();
        });
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            // options.RoutePrefix = string.Empty;
        });

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
