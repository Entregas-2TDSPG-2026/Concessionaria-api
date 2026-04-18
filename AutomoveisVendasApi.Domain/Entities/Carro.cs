namespace AutomoveisVendasApi.Domain.Entities
{
    public class Carro
    {
        public int CarroId { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public int Ano { get; set; }
        public decimal Valor { get; set; }
        public string Placa { get; set; } = string.Empty;
        public bool Vendido { get; set; } = false;
        public List<Venda> Vendas { get; set; } = new();
    }
}
