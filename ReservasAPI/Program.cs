using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReservasAPI.Data;
using ReservasAPI.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ReservasAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ReservasAPIContext") ?? throw new InvalidOperationException("Connection string 'ReservasAPIContext' not found.")));

// Add services to the container.
builder.Services.AddScoped<IServicio,Servicio>();
builder.Services.AddControllers();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
