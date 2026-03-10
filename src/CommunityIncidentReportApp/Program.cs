using Account;
using Common;
using Common.Database;
using CommunityIncidentReportApp.Endpoints;
using Dapper;
using Incidents;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Minio;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureAccountServices(builder.Configuration);
builder.Services.ConfigureIncidentServices();
builder.Services.ConfigureMediator();
builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
    new NpgsqlDbConnectionFactory(builder.Configuration.GetConnectionString("Postgres")!));
builder.Services.AddMinio(options =>
{
    //var httpClientHandler = new HttpClientHandler
    //{
    //    ServerCertificateCustomValidationCallback = ( message, cert, chain, errors ) => true
    //};
    //var httpClient = new HttpClient(httpClientHandler);
    options.WithEndpoint(configuration["Minio:Endpoint"])
        .WithCredentials(configuration["Minio:AccessKey"], configuration["Minio:SecretKey"])
        .WithSSL(false)
        //.WithHttpClient(httpClient)
        .Build();
});
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = configuration["JWT:ValidAudience"],
            ValidIssuer = configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]!))
        };
    });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Inviter", policy => policy.RequireRole("Admin", "SuperAdmin"));
builder.Services.AddAntiforgery();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
    {
        policy.WithOrigins(configuration["ClientUrl"]!)
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowClient");
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapGet("/health", async (IDbConnectionFactory dbConnectionFactory) =>
{
    using var connection = await dbConnectionFactory.CreateConnectionAsync();
    var result = await connection.ExecuteScalarAsync<int>(
        """
            SELECT 1;
        """);
    return Results.Ok($"Database is Online! Result: {result}");
})
.WithName("HealthCheck");
app.MapAccountEndpoints();
app.MapIncidentEndpoints();

app.Run();
