using DevDoListServer.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using DevDoListServer.Data;
using DevDoListServer.Repositories;

var builder = WebApplication.CreateBuilder(args);

var signingKey = Environment.GetEnvironmentVariable("JWT_SECRET");
if(signingKey is null)
{
    throw new Exception("Signing Key does not exist");
}
var jwtOptions = new JwtOptions("https://localhost:7240", "https://localhost:7240", signingKey!, 3600);

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("Connection string does not exist");
}
var dbConnectionDetails = new DbConnectionDetails(connectionString);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(dbConnectionDetails);
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddSingleton(jwtOptions);
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<StatusRepository>();
builder.Services.AddScoped<RoleRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        //convert the string signing key to byte array
        byte[] signingKeyBytes = Encoding.UTF8
            .GetBytes(jwtOptions.SigningKey);

        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler("/error");
app.MapPost("/authenticate", (HttpContext ctx, JwtOptions jwtOptions)
    => TokenEndpoint.Connect(ctx, jwtOptions));

app.MapControllers().RequireAuthorization();

app.Run();
