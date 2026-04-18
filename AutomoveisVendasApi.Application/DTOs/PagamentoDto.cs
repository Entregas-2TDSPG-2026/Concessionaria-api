namespace AutomoveisVendasApi.Application.DTOs
{
    public class PagamentoDto
    {
        public int PagamentoId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataPagamento { get; set; }
    }
}
