using AutomoveisVendasApi.Application.Common;
using AutomoveisVendasApi.Application.DTOs;
using AutomoveisVendasApi.Application.Interfaces;
using AutomoveisVendasApi.Application.Mappings;
using AutomoveisVendasApi.Domain.Entities;
using AutomoveisVendasApi.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace AutomoveisVendasApi.Application.Services
{
    public class VendaService : IVendaService
    {
        private readonly IRepository<Cliente> _clienteRepository;
        private readonly IRepository<Carro> _carroRepository;
        private readonly IRepository<Moto> _motoRepository;
        private readonly IVendaRepository _vendaRepository;
        private readonly ILogger<VendaService> _logger;

        public VendaService(
            IRepository<Cliente> clienteRepository,
            IRepository<Carro> carroRepository,
            IRepository<Moto> motoRepository,
            IVendaRepository vendaRepository,
            ILogger<VendaService> logger)
        {
            _clienteRepository = clienteRepository;
            _carroRepository = carroRepository;
            _motoRepository = motoRepository;
            _vendaRepository = vendaRepository;
            _logger = logger;
        }

        public async Task<VendaDto> CriarVendaAsync(CreateVendaDto dto)
        {
            _logger.LogInformation(
                "Iniciando criação de venda para ClienteId {ClienteId} (CarroId={CarroId}, MotoId={MotoId})",
                dto.ClienteId, dto.CarroId, dto.MotoId);

            var cliente = await _clienteRepository.GetByIdAsync(dto.ClienteId)
                ?? throw new ResourceNotFoundException($"Cliente {dto.ClienteId} não encontrado.");

            var informouCarro = dto.CarroId.HasValue;
            var informouMoto = dto.MotoId.HasValue;

            if (informouCarro == informouMoto)
            {
                throw new DomainException(
                    "A venda deve estar associada a exatamente um veículo: informe CarroId OU MotoId, nunca os dois ou nenhum.");
            }

            Venda venda;
            Carro? carro = null;
            Moto? moto = null;

            if (informouCarro)
            {
                carro = await _carroRepository.GetByIdAsync(dto.CarroId!.Value)
                    ?? throw new ResourceNotFoundException($"Carro {dto.CarroId} não encontrado.");

                if (carro.Vendido)
                    throw new ConflictException($"O carro {dto.CarroId} já foi vendido.");

                venda = Venda.CriarVendaClienteCarro(dto.ClienteId, dto.CarroId.Value, dto.ValorTotal, dto.DataVenda);
            }
            else
            {
                moto = await _motoRepository.GetByIdAsync(dto.MotoId!.Value)
                    ?? throw new ResourceNotFoundException($"Moto {dto.MotoId} não encontrada.");

                if (moto.Vendida)
                    throw new ConflictException($"A moto {dto.MotoId} já foi vendida.");

                venda = Venda.CriarVendaClienteMoto(dto.ClienteId, dto.MotoId.Value, dto.ValorTotal, dto.DataVenda);
            }

            await _vendaRepository.AddAsync(venda);

            if (carro is not null)
            {
                carro.Vendido = true;
                await _carroRepository.UpdateAsync(carro);
            }

            if (moto is not null)
            {
                moto.Vendida = true;
                await _motoRepository.UpdateAsync(moto);
            }

            _logger.LogInformation(
                "Venda {VendaId} criada com sucesso para ClienteId {ClienteId}", venda.VendaId, dto.ClienteId);

            return new VendaDto
            {
                VendaId = venda.VendaId,
                DataVenda = venda.DataVenda,
                ValorTotal = venda.ValorTotal,
                Status = venda.Status,
                Cliente = new ClienteDto
                {
                    ClienteId = cliente.ClienteId,
                    Nome = cliente.Nome,
                    Email = cliente.Email,
                    Telefone = cliente.Telefone
                },
                Carro = carro is null ? null : new CarroDto
                {
                    CarroId = carro.CarroId,
                    Modelo = carro.Modelo,
                    Marca = carro.Marca,
                    Ano = carro.Ano,
                    Valor = carro.Valor,
                    Placa = carro.Placa,
                    Vendido = carro.Vendido
                },
                Moto = moto is null ? null : new MotoDto
                {
                    MotoId = moto.MotoId,
                    Modelo = moto.Modelo,
                    Marca = moto.Marca,
                    Ano = moto.Ano,
                    Valor = moto.Valor,
                    Vendida = moto.Vendida
                },
                Pagamentos = new List<PagamentoDto>()
            };
        }

        public async Task<IReadOnlyList<VendaDto>> ListarAsync()
        {
            var vendas = await _vendaRepository.GetWithDetailsAsync();
            return vendas.Select(VendaMapper.ToDto).ToList();
        }

        public async Task<PagedResult<VendaDto>> ListarPaginadoAsync(long page, int pageSize)
        {
            var request = PageRequest.Create(page, pageSize);
            var resultado = await _vendaRepository.GetPagedAsync(request);
            return resultado.Map(VendaMapper.ToDto);
        }

        public async Task<VendaDto> ObterPorIdAsync(int id)
        {
            var vendas = await _vendaRepository.GetWithDetailsAsync();
            var venda = vendas.FirstOrDefault(v => v.VendaId == id)
                ?? throw new ResourceNotFoundException($"Venda {id} não encontrada.");

            return VendaMapper.ToDto(venda);
        }
    }
}
