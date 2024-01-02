using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RewardService.Data;
using RewardService.Extensions;
using RewardService.Messaging;
using RewardService.Service;
using RewardService.Service.IService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// db connection
builder.Services.AddDbContext<RewardContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnection")));

var optionsBuilder = new DbContextOptionsBuilder<RewardContext>();
optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnection"));
builder.Services.AddSingleton(new RewardsService(optionsBuilder.Options));


// register for di
builder.Services.AddSingleton<IRewardServiceBusConsumer, RewardServiceBusConsumer>();

// configure Auto Mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Extensions

builder.AddSwaggenGenExtension();
builder.AddAuth();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.useAzure();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
