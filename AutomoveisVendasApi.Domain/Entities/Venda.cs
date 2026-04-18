namespace AutomoveisVendasApi.Domain.Entities
{
    public class Venda
    {
        public int VendaId { get; set; }
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }
        public int? CarroId { get; set; }
        public Carro? Carro { get; set; }
        public int? MotoId { get; set; }
        public Moto? Moto { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = "Pendente";
        public List<Pagamento> Pagamentos { get; set; } = new();
    }
}
