using Microsoft.EntityFrameworkCore;
using MyMovieList.Data;
using MyMovieList.Data.Models;
using Microsoft.AspNetCore.Identity;
using MyMovieList.Business.Configuration;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyMovieList.Business.Interfaces.Services;
using MyMovieList.Business.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MyMovieList.HostedServices;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter into field the word 'Bearer' following by space and JWT",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
        });

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,
                            },
                            new List<string>()
                        },
        });
});

builder.Services.AddDbContext<ApiDbContext>(opt =>
{
    if (builder.Configuration.GetValue<bool>("UseSQLite"))
    {
        _ = opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!);
    }
    else
    {
        _ = opt.UseSqlite(builder.Configuration.GetConnectionString("SQLite")!);
    }
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
{
    opt.SignIn.RequireConfirmedAccount = false;

    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApiDbContext>();

builder.Services.AddHostedService<MovieSeedService>();

var corsPolicy = "MyMovieListCorsPolicy";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(name: corsPolicy, policy =>
    {
        policy.WithOrigins(builder.Configuration.GetValue<string>("FrontUrl")!)
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var jwtSection = builder.Configuration.GetSection("JwtBearerTokenSettings");
builder.Services.Configure<JwtBearerTokenSettings>(jwtSection);
var jwtBearerTokenSettings = jwtSection.Get<JwtBearerTokenSettings>()!;
var key = Encoding.ASCII.GetBytes(jwtBearerTokenSettings.SecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = jwtBearerTokenSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtBearerTokenSettings.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero, // To immediately reject the access token
    };
});

var tmdbSection = builder.Configuration.GetSection("Tmdb");
builder.Services.Configure<TmdbApiSettings>(tmdbSection);
var tmdbOptions = tmdbSection.Get<TmdbApiSettings>()!;

builder.Services.AddTransient<SeedData>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IMovieService, MovieService>();
builder.Services.AddTransient<ISeedStateService, SeedStateService>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

using (var serviceScope = app.Services.CreateScope())
{
    var seedData = serviceScope.ServiceProvider.GetService<SeedData>();
    await seedData!.SeedAllDataASync();
}

app.UseHttpsRedirection();

app.UseCors(corsPolicy);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
