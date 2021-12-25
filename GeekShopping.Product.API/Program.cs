using AutoMapper;
using GeekShopping.Product.API.Config;
using GeekShopping.Product.API.Data.ValueObjects;
using GeekShopping.Product.API.Models.Context;
using GeekShopping.Product.API.Repository;
using GeekShopping.Product.API.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration["MySqlConnection:MySQLConnectionString"];

builder.Services.AddDbContext<MySQLContext>(options => options.
UseMySql(connection,
new MySqlServerVersion(
    new Version(8, 0, 27))));

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IProductsRepository, ProductsRepository>();

//builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:4435/";
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false,
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "geek_shopping");
    });
});
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Enter 'Bearer' [space] and your token!",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
      {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = ParameterLocation.Header
        },
        new List<string>()
      }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.MapGet("Products", async (IProductsRepository repository) =>
{
    var products = await repository.FindAll();
    return Results.Ok(products);
}).RequireAuthorization();

app.MapGet("Products/{id}", async (IProductsRepository repository, long id) =>
{
    var product = await repository.FindById(id);
    if (product is null) return Results.NotFound();
    return Results.Ok(product);
}).RequireAuthorization();

app.MapPost("Products", async (IProductsRepository repository, ProductsVO vo) =>
{
    if (vo is null) return Results.BadRequest();
    var product = await repository.Create(vo);
    return Results.Ok(product);
}).RequireAuthorization();

app.MapPut("Products", async (IProductsRepository repository, ProductsVO vo) =>
{
    if (vo is null) return Results.BadRequest();
    var product = await repository.Update(vo);
    return Results.Ok(product);
}).RequireAuthorization();

app.MapDelete("Products/{id}", async (IProductsRepository repository, long id) =>
{
    var status = await repository.Delete(id);
    if (!status) return Results.BadRequest();
    return Results.Ok(status);

}).RequireAuthorization(Role.Admin);

app.UseAuthentication();
app.UseAuthorization();
//app.MapControllers();
app.Run();
