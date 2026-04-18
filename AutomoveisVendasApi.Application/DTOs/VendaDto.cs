namespace AutomoveisVendasApi.Application.DTOs
{
    public class VendaDto
    {
        public int VendaId { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public ClienteDto? Cliente { get; set; }
        public CarroDto? Carro { get; set; }
        public MotoDto? Moto { get; set; }
        public List<PagamentoDto> Pagamentos { get; set; } = new();
    }
}
