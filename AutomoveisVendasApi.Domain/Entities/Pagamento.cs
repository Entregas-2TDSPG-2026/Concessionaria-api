namespace AutomoveisVendasApi.Domain.Entities
{
    public class Pagamento
    {
        public int PagamentoId { get; set; }
        public int VendaId { get; set; }
        public Venda? Venda { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataPagamento { get; set; }
    }
}
