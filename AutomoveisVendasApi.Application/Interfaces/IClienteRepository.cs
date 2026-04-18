using AutomoveisVendasApi.Domain.Entities;

namespace AutomoveisVendasApi.Application.Interfaces
{
    public interface IClienteRepository : IRepository<Cliente>
    {
        Task<Cliente?> GetByEmailAsync(string email);
    }
}
