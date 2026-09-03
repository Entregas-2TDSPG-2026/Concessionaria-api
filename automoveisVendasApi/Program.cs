
using AutomoveisVendasApi.Application.Interfaces;
using AutomoveisVendasApi.Application.Services;
using AutomoveisVendasApi.Infrastructure.Context;
using AutomoveisVendasApi.Infrastructure.Repositories;
using automoveisVendasApi.Exceptions;
using automoveisVendasApi.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ICarroRepository, CarroRepository>();
builder.Services.AddScoped<IMotoRepository, MotoRepository>();
builder.Services.AddScoped<IVendaRepository, VendaRepository>();
builder.Services.AddScoped<IPagamentoRepository, PagamentoRepository>();


builder.Services.AddScoped<IVendaService, VendaService>();

builder.Services.AddSwaggerDocumentation();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}


app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.MapControllers();

app.Run();

public partial class Program { }