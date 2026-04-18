using AutomoveisVendasApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutomoveisVendasApi.Infrastructure.Mappings
{
    public class PagamentoMapping : IEntityTypeConfiguration<Pagamento>
    {
        public void Configure(EntityTypeBuilder<Pagamento> builder)
        {
            builder.ToTable("Pagamentos");
            builder.HasKey(p => p.PagamentoId);
            builder.Property(p => p.PagamentoId).ValueGeneratedOnAdd();
            builder.Property(p => p.VendaId).IsRequired();
            builder.Property(p => p.Tipo).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Valor).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(p => p.DataPagamento).IsRequired();
            builder.HasIndex(p => p.VendaId);
        }
    }
}
