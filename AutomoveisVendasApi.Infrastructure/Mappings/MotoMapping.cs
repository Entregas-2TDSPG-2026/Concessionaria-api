using AutomoveisVendasApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutomoveisVendasApi.Infrastructure.Mappings
{
    public class MotoMapping : IEntityTypeConfiguration<Moto>
    {
        public void Configure(EntityTypeBuilder<Moto> builder)
        {
            builder.ToTable("Motos");
            builder.HasKey(m => m.MotoId);
            builder.Property(m => m.MotoId).ValueGeneratedOnAdd();
            builder.Property(m => m.Modelo).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Marca).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Ano).IsRequired();
            builder.Property(m => m.Valor).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(m => m.Vendida).IsRequired().HasDefaultValue(false);

            builder.HasMany(m => m.Vendas)
                .WithOne(v => v.Moto)
                .HasForeignKey(v => v.MotoId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
