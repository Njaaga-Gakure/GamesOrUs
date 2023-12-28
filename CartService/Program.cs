using CartService.Data;
using CartService.Extensions;
using CartService.Service;
using CartService.Service.IService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Add services to the container.

// db connection
builder.Services.AddDbContext<CartContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnection")));


// configure Auto Mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// tell httpClient about baseURL
builder.Services.AddHttpClient("Product", c => c.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ServiceURL:ProductsBaseURL")));

// register services for DI injection

builder.Services.AddScoped<ICart, CartsService>();
builder.Services.AddScoped<IProduct, ProductService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Extensions
builder.AddAuth();
builder.AddSwaggenGenExtension();

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

app.MapControllers();

app.Run();
