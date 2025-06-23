using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskFlow.Application.DTO.Project;
using TaskFlow.Application.DTO.User;
using TaskFlow.Application.Intefaces.Repositories;
using TaskFlow.Application.Intefaces.Services.JWT;
using TaskFlow.Application.Intefaces.Services.Projects;
using TaskFlow.Application.Intefaces.Services.Users;
using TaskFlow.Application.Intefaces.UnitOfWork;
using TaskFlow.Application.Services.JWT;
using TaskFlow.Application.Services.Projects;
using TaskFlow.Application.Services.Users;
using TaskFlow.Application.Validators.Project;
using TaskFlow.Application.Validators.User;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Persistence.Repositories;
using TaskFlow.Persistence.UnitOfWork;

namespace TaskFlow.API.StartupExtensions;

public static class ConfigurationServiceExtension
{
    public static IServiceCollection ConfigurationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        services.AddOpenApi();

        services.AddHttpContextAccessor();

        #region Inject FluentValidation

        services.AddScoped<IValidator<LoginUserDto>, LoginUserDtoValidator>();
        services.AddScoped<IValidator<CreateUserDto>, CreateUserDtoValidator>();
        services.AddScoped<IValidator<AddProjectRequestDto>, CreateProjectValidator>();
        services.AddScoped<IValidator<UpdateProjectRequestDto>, UpdateProjectValidator>();
        #endregion

        #region Injdect DbContext

        services.AddDbContext<TaskFlowDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        #endregion

        #region Inject Repositories

        services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
        services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
        services.AddScoped(typeof(IProjectRepository), typeof(ProjectRepository));

        #endregion

        #region Inject Services

        services.AddScoped(typeof(IUserAdderService), typeof(UserAdderService));
        services.AddScoped(typeof(IJwtTokenGenerator), typeof(JwtTokenGenerator));
        services.AddScoped(typeof(ICurrentUserService), typeof(CurrentUserService));

        services.AddScoped(typeof(IProjectAdderService), typeof(ProjectAdderService));
        services.AddScoped(typeof(IProjectGetterService), typeof(ProjectGetterService));
        services.AddScoped(typeof(IProjectPoliciesService), typeof(ProjectPoliciesService));
        services.AddScoped(typeof(IProjectUpdaterService), typeof(ProjectUpdaterService));

        #endregion

        #region Jwt Authentication And Authorization

        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                var jwtSettings = configuration.GetSection("JwtSettings");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],

                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Admin");
            });

            options.AddPolicy("UserOnly", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("User");
            });

            options.AddPolicy("ProjectCreators", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Admin", "User");
            });
        });

        #endregion

        return services;
    }
}
