using AutomoveisVendasApi.Domain.Entities;
using AutomoveisVendasApi.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace AutomoveisVendasApi.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Carro> Carros => Set<Carro>();
        public DbSet<Moto> Motos => Set<Moto>();
        public DbSet<Venda> Vendas => Set<Venda>();
        public DbSet<Pagamento> Pagamentos => Set<Pagamento>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ClienteMapping());
            modelBuilder.ApplyConfiguration(new CarroMapping());
            modelBuilder.ApplyConfiguration(new MotoMapping());
            modelBuilder.ApplyConfiguration(new VendaMapping());
            modelBuilder.ApplyConfiguration(new PagamentoMapping());

            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Carro>().HasData(
                new Carro { CarroId = 1, Modelo = "Civic",   Marca = "Honda",  Ano = 2022, Valor = 95000m,  Placa = "ABC-1234", Vendido = true  },
                new Carro { CarroId = 2, Modelo = "Corolla", Marca = "Toyota", Ano = 2023, Valor = 110000m, Placa = "XYZ-5678", Vendido = false }
            );

            modelBuilder.Entity<Moto>().HasData(
                new Moto { MotoId = 1, Modelo = "Ninja 400", Marca = "Kawasaki", Ano = 2022, Valor = 35000m, Vendida = false },
                new Moto { MotoId = 2, Modelo = "CB 500F",   Marca = "Honda",    Ano = 2023, Valor = 30000m, Vendida = false }
            );

            modelBuilder.Entity<Cliente>().HasData(
                new Cliente { ClienteId = 1, Nome = "João Silva",  Email = "joao@email.com",  Telefone = "11999990001" },
                new Cliente { ClienteId = 2, Nome = "Maria Souza", Email = "maria@email.com", Telefone = "11999990002" }
            );

            modelBuilder.Entity<Venda>().HasData(
                new Venda
                {
                    VendaId    = 1,
                    ClienteId  = 1,
                    CarroId    = 1,
                    MotoId     = null,
                    DataVenda  = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc),
                    ValorTotal = 95000m,
                    Status     = "Finalizada"
                }
            );

            modelBuilder.Entity<Pagamento>().HasData(
                new Pagamento
                {
                    PagamentoId   = 1,
                    VendaId       = 1,
                    Tipo          = "Financiamento",
                    Valor         = 95000m,
                    DataPagamento = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
