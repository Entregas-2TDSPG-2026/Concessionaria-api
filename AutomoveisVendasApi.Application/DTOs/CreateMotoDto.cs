
namespace AutomoveisVendasApi.Application.DTOs
{
  
    public class CreateMotoDto
    {
        public string Modelo { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public int Ano { get; set; }
        public decimal Valor { get; set; }
    }
}