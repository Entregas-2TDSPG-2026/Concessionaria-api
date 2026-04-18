using AutomoveisVendasApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutomoveisVendasApi.Infrastructure.Mappings
{
    public class ClienteMapping : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");
            builder.HasKey(c => c.ClienteId);
            builder.Property(c => c.ClienteId).ValueGeneratedOnAdd();
            builder.Property(c => c.Nome).IsRequired().HasMaxLength(150);
            builder.Property(c => c.Email).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Telefone).HasMaxLength(20);
            builder.HasIndex(c => c.Email).IsUnique();

            builder.HasMany(c => c.Vendas)
                .WithOne(v => v.Cliente)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
