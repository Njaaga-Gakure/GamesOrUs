using Microsoft.EntityFrameworkCore;
using RewardService.Data;
using RewardService.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// db connection
builder.Services.AddDbContext<RewardContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnection")));

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

app.UseAuthentication();    
app.UseAuthorization();

app.MapControllers();

app.Run();
