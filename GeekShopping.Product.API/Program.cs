using AutoMapper;
using GeekShopping.Product.API.Config;
using GeekShopping.Product.API.Data.ValueObjects;
using GeekShopping.Product.API.Models.Context;
using GeekShopping.Product.API.Repository;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("Products", async (IProductsRepository repository) =>
{
    var products = await repository.FindAll();
    return Results.Ok(products);
});

app.MapGet("Products/{id}", async (IProductsRepository repository, long id) =>
{
    var product = await repository.FindById(id);
    if (product is null) return Results.NotFound();
    return Results.Ok(product);
});

app.MapPost("Products", async (IProductsRepository repository, ProductsVO vo) =>
{
    if (vo is null) return Results.BadRequest();
    var product = await repository.Create(vo);
    return Results.Ok(product);
});

app.MapPut("Products", async (IProductsRepository repository, ProductsVO vo) =>
{
    if (vo is null) return Results.BadRequest();
    var product = await repository.Update(vo);
    return Results.Ok(product);
});

app.MapDelete("Products/{id}", async (IProductsRepository repository, long id) =>
{
    var status = await repository.Delete(id);
    if (!status) return Results.BadRequest();
    return Results.Ok(status);

});

//app.UseAuthorization();
//app.MapControllers();
app.Run();
