global using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TimeReport.Entities;
using TimeReport.Persistence;
using TimeReport.Shared;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddHttpContextAccessor();

builder.Services.AddFastEndpoints();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddDbContext<TimeReportContext>( options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsHistoryTable("migrations_history"));
});

builder.Services
    .AddIdentity<User, IdentityRole<Guid>>(opt =>
    {
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequiredLength = 6;
        opt.Password.RequireUppercase = false;
        opt.Password.RequireLowercase = false;
        opt.Password.RequireDigit = false;
        opt.Password.RequiredUniqueChars = 1;
    })
    .AddEntityFrameworkStores<TimeReportContext>()
    .AddDefaultTokenProviders();

builder.Services.AddJWTBearerAuth(builder.Configuration["JWT:Key"]!);

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(x => x.Errors.ResponseBuilder = ProblemDetails.ResponseBuilder);

app.Run();