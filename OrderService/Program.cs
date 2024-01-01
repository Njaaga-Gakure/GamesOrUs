using GamesOrUsMessageBus;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Extensions;
using OrderService.Service;
using OrderService.Service.IService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//  db connection

builder.Services.AddDbContext<OrderContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnection")));


// configure Auto Mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Extensions
builder.AddAuth();
builder.AddSwaggenGenExtension();

// stripe config
Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetValue<string>("StripeSecrets:SecretKey");

builder.Services.AddHttpClient("Cart", c => c.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ServiceURL:CartBaseURL")));

// register for di
builder.Services.AddScoped<IOrder, OrdersService>();
builder.Services.AddScoped<ICart, CartService>();
builder.Services.AddScoped<IMessageBus, MessageBus>();


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
