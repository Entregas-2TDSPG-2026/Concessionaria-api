using AutomoveisVendasApi.Application.DTOs;
using AutomoveisVendasApi.Application.Interfaces;
using AutomoveisVendasApi.Infrastructure.Context;
using AutomoveisVendasApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IClienteRepository,   ClienteRepository>();
builder.Services.AddScoped<ICarroRepository,     CarroRepository>();
builder.Services.AddScoped<IMotoRepository,      MotoRepository>();
builder.Services.AddScoped<IVendaRepository,     VendaRepository>();
builder.Services.AddScoped<IPagamentoRepository, PagamentoRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck").WithTags("Health");

app.MapGet("/carros", async (ICarroRepository repo) =>
{
    var carros = await repo.GetAllAsync();
    return Results.Ok(carros.Select(c => new CarroDto
    {
        CarroId = c.CarroId, Modelo = c.Modelo, Marca = c.Marca,
        Ano = c.Ano, Valor = c.Valor, Placa = c.Placa, Vendido = c.Vendido
    }));
}).WithName("GetCarros").WithTags("Carros");

app.MapGet("/carros/disponiveis", async (ICarroRepository repo) =>
{
    var carros = await repo.GetDisponiveisAsync();
    return Results.Ok(carros.Select(c => new CarroDto
    {
        CarroId = c.CarroId, Modelo = c.Modelo, Marca = c.Marca,
        Ano = c.Ano, Valor = c.Valor, Placa = c.Placa, Vendido = c.Vendido
    }));
}).WithName("GetCarrosDisponiveis").WithTags("Carros");

app.MapGet("/motos", async (IMotoRepository repo) =>
{
    var motos = await repo.GetAllAsync();
    return Results.Ok(motos.Select(m => new MotoDto
    {
        MotoId = m.MotoId, Modelo = m.Modelo, Marca = m.Marca,
        Ano = m.Ano, Valor = m.Valor, Vendida = m.Vendida
    }));
}).WithName("GetMotos").WithTags("Motos");

app.MapGet("/motos/disponiveis", async (IMotoRepository repo) =>
{
    var motos = await repo.GetDisponiveisAsync();
    return Results.Ok(motos.Select(m => new MotoDto
    {
        MotoId = m.MotoId, Modelo = m.Modelo, Marca = m.Marca,
        Ano = m.Ano, Valor = m.Valor, Vendida = m.Vendida
    }));
}).WithName("GetMotosDisponiveis").WithTags("Motos");

app.MapGet("/clientes", async (IClienteRepository repo) =>
{
    var clientes = await repo.GetAllAsync();
    return Results.Ok(clientes.Select(c => new ClienteDto
    {
        ClienteId = c.ClienteId, Nome = c.Nome,
        Email = c.Email, Telefone = c.Telefone
    }));
}).WithName("GetClientes").WithTags("Clientes");

app.MapGet("/vendas", async (IVendaRepository repo) =>
{
    var vendas = await repo.GetWithDetailsAsync();
    return Results.Ok(vendas.Select(v => new VendaDto
    {
        VendaId    = v.VendaId,
        DataVenda  = v.DataVenda,
        ValorTotal = v.ValorTotal,
        Status     = v.Status,
        Cliente = v.Cliente is null ? null : new ClienteDto
        {
            ClienteId = v.Cliente.ClienteId, Nome = v.Cliente.Nome,
            Email = v.Cliente.Email, Telefone = v.Cliente.Telefone
        },
        Carro = v.Carro is null ? null : new CarroDto
        {
            CarroId = v.Carro.CarroId, Modelo = v.Carro.Modelo, Marca = v.Carro.Marca,
            Ano = v.Carro.Ano, Valor = v.Carro.Valor, Placa = v.Carro.Placa, Vendido = v.Carro.Vendido
        },
        Moto = v.Moto is null ? null : new MotoDto
        {
            MotoId = v.Moto.MotoId, Modelo = v.Moto.Modelo, Marca = v.Moto.Marca,
            Ano = v.Moto.Ano, Valor = v.Moto.Valor, Vendida = v.Moto.Vendida
        },
        Pagamentos = v.Pagamentos.Select(p => new PagamentoDto
        {
            PagamentoId   = p.PagamentoId,
            Tipo          = p.Tipo,
            Valor         = p.Valor,
            DataPagamento = p.DataPagamento
        }).ToList()
    }));
}).WithName("GetVendas").WithTags("Vendas");

app.Run();
