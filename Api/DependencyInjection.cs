using System.Reflection;
using System.Text;
using Api.Features.Abstractions;
using Api.Persistence;
using Api.Services.Authentication;
using Api.Services.Authorization;
using Api.Services.Behaviors;
using Api.Services.Caching;
using Api.Services.Encryption;
using Api.Services.Exceptions;
using Api.Services.Outbox;
using Api.Services.Time;
using Asp.Versioning;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Quartz;

namespace Api;

internal static class DependencyInjection
{
    public static IServiceCollection AddDependencies(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services
            .MapMediatR()
            .AddApiVersioning()
            .AddExceptionHandlers()
            .AddMapster()
            .AddFluentValidation()
            .AddDatabase(configuration)
            .AddEndpoints(Assembly.GetExecutingAssembly())
            .AddServices()
            .AddAuth(configuration)
            .AddCaching(configuration)
            .SetupQuartz(configuration);
    }

    private static IServiceCollection MapMediatR(this IServiceCollection services)
    {
        _ = services.AddMediatR(config =>
        {
            _ = config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            _ = config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
            _ = config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
            _ = config.AddOpenBehavior(typeof(QueryCachingPipelineBehavior<,>));
        });

        return services;
    }

    private static IServiceCollection AddApiVersioning(this IServiceCollection services)
    {
        _ = services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }

    private static IServiceCollection AddExceptionHandlers(this IServiceCollection services)
    {
        _ = services.AddExceptionHandler<GlobalExceptionHandler>();
        _ = services.AddProblemDetails();

        return services;
    }

    private static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        Assembly assembly
    )
    {
        ServiceDescriptor[] serviceDescriptors =
        [
            .. assembly
                .DefinedTypes.Where(type =>
                    type is { IsAbstract: false, IsInterface: false }
                    && type.IsAssignableTo(typeof(IEndpoint))
                )
                .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type)),
        ];

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    private static IServiceCollection AddMapster(this IServiceCollection services)
    {
        TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;

        _ = config.Scan(Assembly.GetExecutingAssembly());

        _ = services.AddSingleton(config);
        _ = services.AddSingleton<IMapper, ServiceMapper>();

        return services;
    }

    private static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        _ = services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly,
            includeInternalTypes: true
        );

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        _ = services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string? connectionString = configuration.GetConnectionString("Database");

        Ensure.NotNullOrEmpty(connectionString);

        _ = services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(
            new NpgsqlDataSourceBuilder(connectionString).Build()
        ));

        _ = services.AddDbContext<WorkSpaceDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(
                        HistoryRepository.DefaultTableName,
                        Schemas.Default
                    )
            )
        );

        _ = services.AddScoped<IUnitOfWork, WorkSpaceDbContext>();

        return services;
    }

    private static IServiceCollection AddAuth(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        _ = services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        _ = services.AddSingleton<IPasswordHasher, PasswordHasher>();
        _ = services.AddSingleton<IPasswordGenerator, PasswordGenerator>();
        _ = services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        IConfigurationSection jwtSettings = configuration.GetSection("JwtSettings");
        byte[] key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

        _ = services
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false; // Set to true in production
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });

        _ = services.AddAuthorization();

        _ = services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        _ = services.AddTransient<
            IAuthorizationPolicyProvider,
            PermissionAuthorizationPolicyProvider
        >();

        return services;
    }

    private static IServiceCollection AddCaching(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string redisConnectionString = configuration.GetConnectionString("Cache")!;

        _ = services.AddStackExchangeRedisCache(options =>
            options.Configuration = redisConnectionString
        );

        _ = services.AddSingleton<ICacheService, CacheService>();

        return services;
    }

    private static IServiceCollection SetupQuartz(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        _ = services.AddQuartz();
        _ = services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        _ = services.Configure<OutboxOptions>(configuration.GetSection("Outbox"));
        _ = services.ConfigureOptions<ConfigureProcessOutboxJob>();

        return services;
    }
}
