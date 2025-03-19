using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;

using ReviewService.Data;
using ReviewService.Repositories;

var builder = WebApplication.CreateBuilder(args);

var keycloakAuthority = builder.Configuration["Keycloak:Authority"];
var keycloakExternalAuthority = builder.Configuration["Keycloak:ExternalAuthority"] ?? keycloakAuthority;
var keycloakClientId = builder.Configuration["Keycloak:ClientId"];

builder.Services.AddDbContext<ReviewDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.Authority = keycloakAuthority;
        options.Audience = keycloakClientId;
        options.RequireHttpsMetadata = false;
        
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidIssuers = [keycloakAuthority, keycloakExternalAuthority],
            ValidateAudience = true,
            ValidAudience = keycloakClientId,
            ValidateLifetime = true
        };
    });

builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "Review Service API", 
        Version = "v1",
        Description = "API for managing book reviews"
    });

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows {
            AuthorizationCode = new OpenApiOAuthFlow {
                AuthorizationUrl = new Uri($"{keycloakExternalAuthority}/protocol/openid-connect/auth"),
                TokenUrl = new Uri($"{keycloakExternalAuthority}/protocol/openid-connect/token"),
                Scopes = new Dictionary<string, string> {
                    { "openid", "OpenID Connect" },
                    { "profile", "User profile" },
                    { "email", "User email" }
                }
            }
        }
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new List<string> { "openid", "profile", "email" }
        }
    });
});

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    var dbContext = scope.ServiceProvider.GetRequiredService<ReviewDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(options => {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Review Service API v1");
    options.OAuthClientId(keycloakClientId);
    options.OAuthAppName("Review Service - Swagger");
    options.OAuthUsePkce();
});
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
