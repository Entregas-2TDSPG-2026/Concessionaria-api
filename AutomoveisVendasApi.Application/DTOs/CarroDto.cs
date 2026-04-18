namespace AutomoveisVendasApi.Application.DTOs
{
    public class CarroDto
    {
        public int CarroId { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public int Ano { get; set; }
        public decimal Valor { get; set; }
        public string Placa { get; set; } = string.Empty;
        public bool Vendido { get; set; }
    }
}
