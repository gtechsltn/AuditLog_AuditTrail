using System.Text;
using AuditTrails.Configuration;
using AuditTrails.Database;
using AuditTrails.Mapping;
using AuditTrails.Services;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Postgres");

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentSessionProvider, CurrentSessionProvider>();
builder.Services.AddSingleton<AuditableInterceptor>();

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();

builder.Services.AddDbContext<ApplicationDbContext>((provider, options) =>
{
	var interceptor = provider.GetRequiredService<AuditableInterceptor>();

    options.EnableSensitiveDataLogging()
	    .UseNpgsql(dataSourceBuilder.Build(), npgsqlOptions =>
	    {
		    npgsqlOptions.MigrationsHistoryTable("__MyMigrationsHistory", "devtips_audit_trails");
	    })
	    .AddInterceptors(interceptor)
	    .UseSnakeCaseNamingConvention();
});

builder.Services.AddOptions<AuthConfiguration>()
	.Bind(builder.Configuration.GetSection(nameof(AuthConfiguration)));

builder.Services.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["AuthConfiguration:Issuer"],
			ValidAudience = builder.Configuration["AuthConfiguration:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AuthConfiguration:Key"]!))
		};
	});

builder.Services.AddCarter();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();

// Create and seed database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DatabaseSeedService.SeedAsync(dbContext);
}

await app.RunAsync();
