using AutomoveisVendasApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutomoveisVendasApi.Infrastructure.Mappings
{
    public class CarroMapping : IEntityTypeConfiguration<Carro>
    {
        public void Configure(EntityTypeBuilder<Carro> builder)
        {
            builder.ToTable("Carros");
            builder.HasKey(c => c.CarroId);
            builder.Property(c => c.CarroId).ValueGeneratedOnAdd();
            builder.Property(c => c.Modelo).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Marca).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Ano).IsRequired();
            builder.Property(c => c.Valor).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(c => c.Placa).IsRequired().HasMaxLength(10);
            builder.Property(c => c.Vendido).IsRequired().HasDefaultValue(false);
            builder.HasIndex(c => c.Placa).IsUnique();

            builder.HasMany(c => c.Vendas)
                .WithOne(v => v.Carro)
                .HasForeignKey(v => v.CarroId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
