
namespace AutomoveisVendasApi.Application.DTOs
{
   
    public class CreatePagamentoDto
    {
        public int VendaId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataPagamento { get; set; } = DateTime.UtcNow;
    }
}