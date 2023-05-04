using CodeFirstRestaurantAPI.Models;
using CodeFirstRestaurantAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// passing connection string..........
builder.Services.AddDbContext<RestaurantAppContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("RestaurantConn"));
});

// we are using for every session new object gets created ->scopedlifetime. 
// we are using for every requests ->transientlifetime.
builder.Services.AddScoped<IService<Menu, int>, MenuService>();

builder.Services.AddScoped<ICategoryService<Category, int>, CategoryService>();



builder.Services.AddCors();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder =>
{
    builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
