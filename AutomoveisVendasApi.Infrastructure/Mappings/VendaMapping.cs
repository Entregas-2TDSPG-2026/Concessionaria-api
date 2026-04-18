using AutomoveisVendasApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutomoveisVendasApi.Infrastructure.Mappings
{
    public class VendaMapping : IEntityTypeConfiguration<Venda>
    {
        public void Configure(EntityTypeBuilder<Venda> builder)
        {
            builder.ToTable("Vendas");
            builder.HasKey(v => v.VendaId);
            builder.Property(v => v.VendaId).ValueGeneratedOnAdd();
            builder.Property(v => v.ClienteId).IsRequired();
            builder.Property(v => v.CarroId).IsRequired(false);
            builder.Property(v => v.MotoId).IsRequired(false);
            builder.Property(v => v.DataVenda).IsRequired();
            builder.Property(v => v.ValorTotal).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(v => v.Status).IsRequired().HasMaxLength(50).HasDefaultValue("Pendente");

            builder.HasIndex(v => v.ClienteId);
            builder.HasIndex(v => v.CarroId);
            builder.HasIndex(v => v.MotoId);

            builder.HasMany(v => v.Pagamentos)
                .WithOne(p => p.Venda)
                .HasForeignKey(p => p.VendaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
