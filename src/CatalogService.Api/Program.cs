using CatalogService.Api.Middleware;
using CatalogService.Application.Brands;
using CatalogService.Application.Categories;
using CatalogService.Application.Common.Behaviors;
using CatalogService.Application.ProductImages;
using CatalogService.Application.Products;
using CatalogService.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var catalogConnectionString = builder.Configuration.GetConnectionString("Catalog") ?? string.Empty;
var jwtSigningKey = builder.Configuration["Jwt:SigningKey"] ?? string.Empty;
if (!builder.Environment.IsDevelopment() && string.IsNullOrWhiteSpace(catalogConnectionString))
{
    throw new InvalidOperationException("Catalog connection string must be configured outside development.");
}

if (!builder.Environment.IsDevelopment() && string.IsNullOrWhiteSpace(jwtSigningKey))
{
    throw new InvalidOperationException("Jwt:SigningKey must be configured outside development.");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCatalogPersistence(builder.Configuration);
builder.Services.AddScoped<IValidator<CreateCategoryCommand>, CreateCategoryCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateCategoryCommand>, UpdateCategoryCommandValidator>();
builder.Services.AddScoped<IValidator<CreateBrandCommand>, CreateBrandCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateBrandCommand>, UpdateBrandCommandValidator>();
builder.Services.AddScoped<IValidator<GetProductsQuery>, GetProductsQueryValidator>();
builder.Services.AddScoped<IValidator<CreateProductCommand>, CreateProductCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateProductCommand>, UpdateProductCommandValidator>();
builder.Services.AddScoped<IValidator<AddProductImageCommand>, AddProductImageCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? ["http://localhost:3000"];

        policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var signingKey = builder.Configuration["Jwt:SigningKey"] ?? "7Xlcw53AYw6VmPpz0etfkX41+qiBJ4t14ZLQ5D/mSu1kQC0dc7rOt+JbMhOW/1Um";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsEnvironment("Container"))
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<ApiExceptionMiddleware>();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
