using AutomoveisVendasApi.Domain.Entities;
using AutomoveisVendasApi.Domain.Exceptions;
using Xunit;

namespace AutomoveisVendasApi.Domain.Tests
{
    public class VendaTests
    {
        [Fact]
        public void CriarVendaClienteCarro_DadosValidos_CriaVendaComStatusPendente()
        {
            var clienteId = 1;
            var carroId = 10;
            var valorTotal = 95000m;
            var dataVenda = DateTime.UtcNow;

            var venda = Venda.CriarVendaClienteCarro(clienteId, carroId, valorTotal, dataVenda);

            Assert.Equal(clienteId, venda.ClienteId);
            Assert.Equal(carroId, venda.CarroId);
            Assert.Null(venda.MotoId);
            Assert.Equal(valorTotal, venda.ValorTotal);
            Assert.Equal("Pendente", venda.Status);
        }

        [Fact]
        public void CriarVendaClienteMoto_DadosValidos_CriaVendaComStatusPendente()
        {
            var clienteId = 2;
            var motoId = 5;
            var valorTotal = 35000m;
            var dataVenda = DateTime.UtcNow;

            var venda = Venda.CriarVendaClienteMoto(clienteId, motoId, valorTotal, dataVenda);

            Assert.Equal(clienteId, venda.ClienteId);
            Assert.Equal(motoId, venda.MotoId);
            Assert.Null(venda.CarroId);
            Assert.Equal(valorTotal, venda.ValorTotal);
            Assert.Equal("Pendente", venda.Status);
        }

        [Theory]
        [InlineData(0, 10, 95000)]
        [InlineData(1, 0, 95000)]
        [InlineData(1, 10, 0)]
        [InlineData(1, 10, -1000)]
        public void CriarVendaClienteCarro_DadosInvalidos_LancaDomainException(
            int clienteId, int carroId, decimal valorTotal)
        {
            var dataVenda = DateTime.UtcNow;

            Assert.Throws<DomainException>(() =>
                Venda.CriarVendaClienteCarro(clienteId, carroId, valorTotal, dataVenda));
        }

        [Fact]
        public void CriarVendaClienteCarro_DataVendaNoFuturo_LancaDomainException()
        {
            var dataVendaFutura = DateTime.UtcNow.AddDays(1);

            Assert.Throws<DomainException>(() =>
                Venda.CriarVendaClienteCarro(1, 10, 95000m, dataVendaFutura));
        }

        [Fact]
        public void Finalizar_VendaPendente_AlteraStatusParaFinalizada()
        {
            var venda = Venda.CriarVendaClienteCarro(1, 10, 95000m, DateTime.UtcNow);

            venda.Finalizar();

            Assert.Equal("Finalizada", venda.Status);
        }

        [Fact]
        public void Finalizar_VendaJaFinalizada_LancaDomainException()
        {
            var venda = Venda.CriarVendaClienteCarro(1, 10, 95000m, DateTime.UtcNow);
            venda.Finalizar();

            Assert.Throws<DomainException>(() => venda.Finalizar());
        }
    }
}
