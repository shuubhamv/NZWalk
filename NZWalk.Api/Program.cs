using Microsoft.AspNetCore.Authentication.JwtBearer;  // Enables JWT authentication, allowing secure access to API endpoints
using Microsoft.AspNetCore.Identity; //Provides user authentication & authorization functionalities.
using Microsoft.EntityFrameworkCore;//Provides an ORM (Object-Relational Mapping) tool for database operations
using Microsoft.Extensions.Options;//Provides configuration management (e.g., appsettings.json values)
using Microsoft.IdentityModel.Tokens;//  Used for JWT token validation.
using NZWalk.Api.Data; //Imports database context classes (NZWalksDbContext, NZWalksAuthDbContext).
using NZWalk.Api.Mapping;  // Imports AutoMapper configuration.
using NZWalk.Api.Repositories; // Imports repository interfaces and implementations.
using System.Text; //Provides text manipulation functions.   Used in JWT token encoding (converts string to byte[])
using Microsoft.OpenApi.Models;  //Supports Swagger API documentation.
using System.Net.NetworkInformation; //Provides network information retrieval functions.
using Microsoft.Extensions.FileProviders;//Enables serving static files (e.g., images).Concept: Static File Middleware – Serves images securely.
using Serilog;// Provides structured logging //: Logging Middleware – Stores logs for debugging
using NZWalk.Api.Middlewares; //imports custom exception-handling middleware

var builder = WebApplication.CreateBuilder(args);            //Creates an instance of WebApplication to configure services & middleware.

// Add services to the container.

//serilog injection
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/NZWalks_log.txt",rollingInterval:RollingInterval.Day)
    .MinimumLevel.Warning() 
    .CreateLogger();

builder.Logging.ClearProviders();   // remove default logging
builder.Logging.AddSerilog(logger); // add serilog
// end  seilog


builder.Services.AddControllers();  //Registers API Controllers.

builder.Services.AddHttpContextAccessor(); // Required for handling file uploads (e.g., images)which allows access to HTTP context (such as requests, user identity, and session data) f

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();   //Registers the API Explorer service, which is required for Swagger to discover API endpoints.
//This enables automatic documentation for API endpoints.

builder.Services.AddSwaggerGen(Options =>
{
    Options.SwaggerDoc("v1", new() { Title = "NZWalk.Api", Version = "v1" });

    Options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",   // Uses the "Authorization" header to send JWT tokens.
        In = ParameterLocation.Header,   //The token is passed in the request header
        Type = SecuritySchemeType.ApiKey,  //Tells Swagger this is an API key authentication method
        Scheme = JwtBearerDefaults.AuthenticationScheme,  //Uses "Bearer" as the scheme
    });

    // Forces Swagger to Require JWT Authentication
    Options.AddSecurityRequirement(new OpenApiSecurityRequirement
{
        {
          new OpenApiSecurityScheme
          {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
             Scheme = "oauth2",  //"oauth2" is the security scheme, but in this case, we use it for JWT tokens.
              Name = JwtBearerDefaults.AuthenticationScheme,
               In = ParameterLocation.Header,
          },
             new List<string>()  //Applies to all endpoints (empty list new List<string>() means it's global).

        }
    });
});

builder.Services.AddDbContext<NZWalksDbContext>(Options =>
Options.UseSqlServer(builder.Configuration.GetConnectionString("NZWalksConnectionString")));

builder.Services.AddDbContext<NZWalksAuthDbContext>(Options =>
Options.UseSqlServer(builder.Configuration.GetConnectionString("NZWalksAuthConnectionString")));

//Implements Repository Pattern for database access.
//builder.Services ? Accesses the service collection where dependencies are registered.
//AddScoped<TService, TImplementation>() ? Registers a service with a scoped lifetime.
//IRegionRepository ? The interface that defines the contract for data access.
//SqlRegionRepository ? The concrete implementation of IRegionRepository.

builder.Services.AddScoped<IRegionRepository, SqlRegionRepository>();
builder.Services.AddScoped<IWalkRepository, SqlWalkRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IImageRepository, LocalImageRepository>();

builder.Services.AddAutoMapper(typeof(AutomapperProfiles));

//Manages Users & Roles using Identity.
//registers ASP.NET Core Identity with the base user model (IdentityUser).

//IdentityUser ? A built-in class representing user accounts(with properties like UserName, Email, PasswordHash, etc.).
//Unlike AddIdentity<TUser, TRole>(), this registers only the user system (without UI features like login pages).
//?? Why use AddIdentityCore<TUser>() instead of AddIdentity<TUser, TRole>()?

//AddIdentityCore is lightweight and ideal for APIs.
//AddIdentity is used when both users & roles are required with UI support.

builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("NZWalks")  //Registers a token provider for generating security tokens.
    .AddEntityFrameworkStores<NZWalksAuthDbContext>()  //Configures ASP.NET Core Identity to use Entity Framework Core for storing users and roles in the database.
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    //options.User.RequireUniqueEmail = true;
    options.Password.RequiredUniqueChars = 1 ;
});

//Enables JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    });


//Builds the app after configuring all services.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//Sets up middleware for error handling, authentication, authorization, and static files.
app.UseMiddleware<ExceptionHandllerMidlware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseStaticFiles( new StaticFileOptions
{
    FileProvider= new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
    RequestPath= "/Images"

    //Https://Loacalost:1234/Images 
} );

app.MapControllers();

app.Run();
