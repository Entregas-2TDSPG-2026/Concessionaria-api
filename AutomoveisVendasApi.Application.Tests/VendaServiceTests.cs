using AutomoveisVendasApi.Application.DTOs;
using AutomoveisVendasApi.Application.Interfaces;
using AutomoveisVendasApi.Application.Services;
using AutomoveisVendasApi.Domain.Entities;
using AutomoveisVendasApi.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AutomoveisVendasApi.Application.Tests
{
    public class VendaServiceTests
    {
        private readonly Mock<IRepository<Cliente>> _clienteRepositoryMock = new();
        private readonly Mock<IRepository<Carro>> _carroRepositoryMock = new();
        private readonly Mock<IRepository<Moto>> _motoRepositoryMock = new();
        private readonly Mock<IRepository<Venda>> _vendaRepositoryMock = new();
        private readonly Mock<ILogger<VendaService>> _loggerMock = new();

        private VendaService CriarService() => new(
            _clienteRepositoryMock.Object,
            _carroRepositoryMock.Object,
            _motoRepositoryMock.Object,
            _vendaRepositoryMock.Object,
            _loggerMock.Object);

        [Fact]
        public async Task CriarVendaAsync_ClienteInexistente_LancaResourceNotFoundException_ENaoPersiste()
        {
            _clienteRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Cliente?)null);

            var dto = new CreateVendaDto { ClienteId = 99, CarroId = 1, ValorTotal = 50000m, DataVenda = DateTime.UtcNow };
            var service = CriarService();

            await Assert.ThrowsAsync<ResourceNotFoundException>(() => service.CriarVendaAsync(dto));

            _vendaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Venda>()), Times.Never);
        }

        [Fact]
        public async Task CriarVendaAsync_CarroInexistente_LancaResourceNotFoundException_ENaoPersiste()
        {
            _clienteRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Cliente { ClienteId = 1, Nome = "Cliente Teste", Email = "teste@email.com" });

            _carroRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Carro?)null);

            var dto = new CreateVendaDto { ClienteId = 1, CarroId = 123, ValorTotal = 50000m, DataVenda = DateTime.UtcNow };
            var service = CriarService();

            await Assert.ThrowsAsync<ResourceNotFoundException>(() => service.CriarVendaAsync(dto));

            _vendaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Venda>()), Times.Never);
        }

        [Fact]
        public async Task CriarVendaAsync_CarroJaVendido_LancaConflictException_ENaoPersiste()
        {
            _clienteRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Cliente { ClienteId = 1, Nome = "Cliente Teste", Email = "teste@email.com" });

            _carroRepositoryMock
                .Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(new Carro { CarroId = 10, Modelo = "Civic", Marca = "Honda", Vendido = true });

            var dto = new CreateVendaDto { ClienteId = 1, CarroId = 10, ValorTotal = 95000m, DataVenda = DateTime.UtcNow };
            var service = CriarService();

            await Assert.ThrowsAsync<ConflictException>(() => service.CriarVendaAsync(dto));

            _vendaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Venda>()), Times.Never);
        }

        [Fact]
        public async Task CriarVendaAsync_CarroEMotoInformadosJuntos_LancaDomainException_ENaoPersiste()
        {
            _clienteRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Cliente { ClienteId = 1, Nome = "Cliente Teste", Email = "teste@email.com" });

            var dto = new CreateVendaDto { ClienteId = 1, CarroId = 10, MotoId = 5, ValorTotal = 95000m, DataVenda = DateTime.UtcNow };
            var service = CriarService();

            await Assert.ThrowsAsync<DomainException>(() => service.CriarVendaAsync(dto));

            _vendaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Venda>()), Times.Never);
        }

        [Fact]
        public async Task CriarVendaAsync_DadosValidosComCarro_PersisteVendaUmaVezEAtualizaCarro()
        {
            var cliente = new Cliente { ClienteId = 1, Nome = "Cliente Teste", Email = "teste@email.com" };
            var carro = new Carro { CarroId = 10, Modelo = "Civic", Marca = "Honda", Ano = 2022, Valor = 95000m, Vendido = false };

            _clienteRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cliente);
            _carroRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(carro);
            _vendaRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Venda>()))
                .Returns(Task.CompletedTask);

            var dto = new CreateVendaDto { ClienteId = 1, CarroId = 10, ValorTotal = 95000m, DataVenda = DateTime.UtcNow };
            var service = CriarService();

            var resultado = await service.CriarVendaAsync(dto);

            Assert.Equal("Pendente", resultado.Status);
            Assert.NotNull(resultado.Carro);
            Assert.True(carro.Vendido);

            _vendaRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Venda>()), Times.Once);
            _carroRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Carro>(c => c.Vendido)), Times.Once);
        }
    }
}
