using AutomoveisVendasApi.Application.Interfaces;
using AutomoveisVendasApi.Domain.Entities;
using AutomoveisVendasApi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AutomoveisVendasApi.Infrastructure.Repositories
{
    public class CarroRepository : Repository<Carro>, ICarroRepository
    {
        public CarroRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Carro>> GetDisponiveisAsync() =>
            await _dbSet.Where(c => !c.Vendido).ToListAsync();

        public async Task<Carro?> GetByPlacaAsync(string placa) =>
            await _dbSet.FirstOrDefaultAsync(c => c.Placa == placa);
    }
}
