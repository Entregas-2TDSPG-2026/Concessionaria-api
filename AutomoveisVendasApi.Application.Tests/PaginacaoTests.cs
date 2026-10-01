using AutomoveisVendasApi.Application.Common;
using AutomoveisVendasApi.Application.Interfaces;
using AutomoveisVendasApi.Application.Services;
using AutomoveisVendasApi.Domain.Entities;
using AutomoveisVendasApi.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AutomoveisVendasApi.Application.Tests
{
    public class PaginacaoTests
    {
        private readonly Mock<IVendaRepository> _vendaRepositoryMock = new();

        private VendaService CriarService() => new(
            new Mock<IRepository<Cliente>>().Object,
            new Mock<IRepository<Carro>>().Object,
            new Mock<IRepository<Moto>>().Object,
            _vendaRepositoryMock.Object,
            new Mock<ILogger<VendaService>>().Object);

        [Theory]
        [InlineData(0, 20)]
        [InlineData(-1, 20)]
        [InlineData(1, 0)]
        [InlineData(1, -5)]
        [InlineData(1, 101)]
        [InlineData(1, 9999)]
        public async Task ListarPaginadoAsync_PageOuPageSizeInvalido_LancaDomainException_ENaoConsultaRepositorio(
            int page, int pageSize)
        {
            var service = CriarService();

            var ex = await Assert.ThrowsAsync<DomainException>(() => service.ListarPaginadoAsync(page, pageSize));

            Assert.Contains("page", ex.Message);
            _vendaRepositoryMock.Verify(r => r.GetPagedAsync(It.IsAny<PageRequest>()), Times.Never);
        }

        [Fact]
        public async Task ListarPaginadoAsync_IntervaloValido_DevolveEnvelopeComTotais()
        {
            var venda = Venda.CriarVendaClienteCarro(1, 1, 50000m, DateTime.UtcNow);
            _vendaRepositoryMock
                .Setup(r => r.GetPagedAsync(It.Is<PageRequest>(p => p.Page == 2 && p.PageSize == 2)))
                .ReturnsAsync(PagedResult<Venda>.Create(new[] { venda }, page: 2, pageSize: 2, totalItems: 5));

            var resultado = await CriarService().ListarPaginadoAsync(2, 2);

            Assert.Equal(2, resultado.Page);
            Assert.Equal(2, resultado.PageSize);
            Assert.Equal(5, resultado.TotalItems);
            Assert.Equal(3, resultado.TotalPages);   // teto de 5 / 2
            Assert.Single(resultado.Items);
            Assert.True(resultado.HasPrevious);
            Assert.True(resultado.HasNext);
        }

        [Fact]
        public void PageRequest_PageEnorme_NaoEstouraOffset()
        {
            var request = PageRequest.Create(long.MaxValue, 100);

            Assert.Equal(long.MaxValue, request.Offset);
        }
    }
}