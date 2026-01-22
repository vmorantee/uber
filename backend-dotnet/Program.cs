using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RideSharingAPI.Data;
using RideSharingAPI.Hubs;
using RideSharingAPI.Repositories;
using RideSharingAPI.Services;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

var elasticsearchUri = Environment.GetEnvironmentVariable("Elasticsearch__Uri") 
    ?? builder.Configuration["Elasticsearch:Uri"] 
    ?? "http://localhost:9200";

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithProperty("Application", "RideSharingAPI")
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchUri))
    {
        IndexFormat = $"ridesharing-api-{DateTime.UtcNow:yyyy-MM}",
        AutoRegisterTemplate = true,
        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
        FailureCallback = (e, ex) => Console.WriteLine($"Unable to submit event: {e.MessageTemplate}. Exception: {ex?.Message}"),
        EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                           EmitEventFailureHandling.RaiseCallback
    })
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
    ?? builder.Configuration["Jwt:Key"] 
    ?? "RideSharingSecretKey2026SuperSecure!AtLeast32Chars";
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") 
    ?? builder.Configuration["Jwt:Issuer"] 
    ?? "RideSharingAPI";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") 
    ?? builder.Configuration["Jwt:Audience"] 
    ?? "RideSharingClients";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddDbContext<RideSharingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSignalR();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IRideRepository, RideRepository>();
builder.Services.AddScoped<IBannerRepository, BannerRepository>();
builder.Services.AddScoped<IDriverLocationRepository, DriverLocationRepository>();
builder.Services.AddSingleton<IServiceBusSender, ServiceBusSender>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RideSharingContext>();
    try
    {
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Database migration skipped or failed");
    }
}

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
    };
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<RideHub>("/hubs/ride");

Log.Information("RideSharing API started. Elasticsearch: {ElasticsearchUri}", elasticsearchUri);

app.Run();
