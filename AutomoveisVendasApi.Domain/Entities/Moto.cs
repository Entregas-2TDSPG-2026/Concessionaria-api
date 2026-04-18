namespace AutomoveisVendasApi.Domain.Entities
{
    public class Moto
    {
        public int MotoId { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public int Ano { get; set; }
        public decimal Valor { get; set; }
        public bool Vendida { get; set; } = false;
        public List<Venda> Vendas { get; set; } = new();
    }
}
