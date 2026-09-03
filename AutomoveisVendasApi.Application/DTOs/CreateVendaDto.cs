// AutomoveisVendasApi.Application/DTOs/CreateVendaDto.cs
namespace AutomoveisVendasApi.Application.DTOs
{
    public class CreateVendaDto
    {
        public int ClienteId { get; set; }
        public int? CarroId { get; set; }
        public int? MotoId { get; set; }
        public decimal ValorTotal { get; set; }
        public DateTime DataVenda { get; set; } = DateTime.UtcNow;
    }
}