using AutomoveisVendasApi.Domain.Entities;

namespace AutomoveisVendasApi.Application.Interfaces
{
    public interface IMotoRepository : IRepository<Moto>
    {
        Task<IEnumerable<Moto>> GetDisponiveisAsync();
    }
}
