using AutoMapper;
using GeekShopping.CartAPI.Config;
using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Models.Context;
using GeekShopping.CartAPI.Repository;
using Microsoft.AspNetCore.Mvc;
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
builder.Services.AddScoped<ICartRepository, CartRepository>();

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

string basePath = "api/v1/cart/";

app.MapGet(basePath + "find-cart/{userId}", async ([FromServices]ICartRepository cartRepository, string userId) =>
{
    var cart = await cartRepository.FindCartByUserId(userId);
    if (cart is null) return Results.NotFound();
    return Results.Ok(cart);
});

app.MapPost(basePath + "add-cart", async ([FromServices]ICartRepository cartRepository, CartVO vo) =>
{
    var cart = await cartRepository.SaveOrUpdateCart(vo);
    if (cart is null) return Results.NotFound();
    return Results.Ok(cart);
});

app.MapPut(basePath + "update-cart", async ([FromServices]ICartRepository cartRepository, CartVO vo) =>
{
    var cart = await cartRepository.SaveOrUpdateCart(vo);
    if (cart is null) return Results.NotFound();
    return Results.Ok(cart); 
});

app.MapDelete(basePath + "remove-cart/{id}", async ([FromServices]ICartRepository cartRepository, int id) =>
{
    var status = await cartRepository.RemoveFromCart(id);
    if (!status) return Results.BadRequest();
    return Results.Ok(status);
});

app.MapPost(basePath + "apply-coupon", async ([FromServices] ICartRepository cartRepository, CartVO vo) =>
{
    var status = await cartRepository.ApplyCoupon(vo.CartHeader.UserId, vo.CartHeader.CouponCode);
    if (!status) return Results.NotFound();
    return Results.Ok(status);
});

app.MapDelete(basePath + "remove-coupon/{userId}", async ([FromServices] ICartRepository cartRepository, string userId) =>
{
    var status = await cartRepository.RemoveCoupon(userId);
    if (!status) return Results.NotFound();
    return Results.Ok(status);
});


app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();


app.Run();
