using AutomoveisVendasApi.Domain.Exceptions;

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

       
        public Venda() { }

       
        public static Venda CriarVendaClienteCarro(int clienteId, int carroId, decimal valorTotal, DateTime dataVenda)
        {
            ValidarDadosComuns(clienteId, valorTotal, dataVenda);

            if (carroId <= 0)
                throw new DomainException("O identificador do carro deve ser maior que zero.");

            return new Venda
            {
                ClienteId = clienteId,
                CarroId = carroId,
                MotoId = null,
                ValorTotal = valorTotal,
                DataVenda = dataVenda,
                Status = "Pendente"
            };
        }

      
        public static Venda CriarVendaClienteMoto(int clienteId, int motoId, decimal valorTotal, DateTime dataVenda)
        {
            ValidarDadosComuns(clienteId, valorTotal, dataVenda);

            if (motoId <= 0)
                throw new DomainException("O identificador da moto deve ser maior que zero.");

            return new Venda
            {
                ClienteId = clienteId,
                MotoId = motoId,
                CarroId = null,
                ValorTotal = valorTotal,
                DataVenda = dataVenda,
                Status = "Pendente"
            };
        }

      
        public void Finalizar()
        {
            if (Status != "Pendente")
                throw new DomainException($"Só é possível finalizar vendas com status 'Pendente'. Status atual: '{Status}'.");

            Status = "Finalizada";
        }

        private static void ValidarDadosComuns(int clienteId, decimal valorTotal, DateTime dataVenda)
        {
            if (clienteId <= 0)
                throw new DomainException("O identificador do cliente deve ser maior que zero.");

            if (valorTotal <= 0)
                throw new DomainException("O valor total da venda deve ser maior que zero.");

            if (dataVenda > DateTime.UtcNow.AddMinutes(5))
                throw new DomainException("A data da venda não pode estar no futuro.");
        }
    }
}