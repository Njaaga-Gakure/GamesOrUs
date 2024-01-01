using CouponService.Data;
using Microsoft.EntityFrameworkCore;
using CouponService.Extensions;
using CouponService.Service.IService;
using CouponService.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// db connection
builder.Services.AddDbContext<CouponContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnection")));

// configure Auto Mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// register for di injection

builder.Services.AddScoped<ICoupon, CouponsService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Extensions
builder.AddAuth();
builder.AddSwaggenGenExtension();

// stripe config
Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetValue<string>("StripeSecrets:SecretKey");

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
