using AutomoveisVendasApi.Domain.Entities;

namespace AutomoveisVendasApi.Application.Interfaces
{
    public interface ICarroRepository : IRepository<Carro>
    {
        Task<IEnumerable<Carro>> GetDisponiveisAsync();
        Task<Carro?> GetByPlacaAsync(string placa);
    }
}
