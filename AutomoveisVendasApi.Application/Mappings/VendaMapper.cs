using AutomoveisVendasApi.Application.DTOs;
using AutomoveisVendasApi.Domain.Entities;

namespace AutomoveisVendasApi.Application.Mappings
{
    /// <summary>Entidade -> DTO. Compartilhado por v1 e v2 (o JSON de cada item é o mesmo).</summary>
    public static class VendaMapper
    {
        public static VendaDto ToDto(Venda v) => new()
        {
            VendaId = v.VendaId,
            DataVenda = v.DataVenda,
            ValorTotal = v.ValorTotal,
            Status = v.Status,
            Cliente = v.Cliente is null ? null : new ClienteDto
            {
                ClienteId = v.Cliente.ClienteId,
                Nome = v.Cliente.Nome,
                Email = v.Cliente.Email,
                Telefone = v.Cliente.Telefone
            },
            Carro = v.Carro is null ? null : new CarroDto
            {
                CarroId = v.Carro.CarroId,
                Modelo = v.Carro.Modelo,
                Marca = v.Carro.Marca,
                Ano = v.Carro.Ano,
                Valor = v.Carro.Valor,
                Placa = v.Carro.Placa,
                Vendido = v.Carro.Vendido
            },
            Moto = v.Moto is null ? null : new MotoDto
            {
                MotoId = v.Moto.MotoId,
                Modelo = v.Moto.Modelo,
                Marca = v.Moto.Marca,
                Ano = v.Moto.Ano,
                Valor = v.Moto.Valor,
                Vendida = v.Moto.Vendida
            },
            Pagamentos = v.Pagamentos.Select(p => new PagamentoDto
            {
                PagamentoId = p.PagamentoId,
                Tipo = p.Tipo,
                Valor = p.Valor,
                DataPagamento = p.DataPagamento
            }).ToList()
        };
    }
}