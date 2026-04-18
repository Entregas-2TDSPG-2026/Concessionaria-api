using AutomoveisVendasApi.Application.Interfaces;
using AutomoveisVendasApi.Domain.Entities;
using AutomoveisVendasApi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AutomoveisVendasApi.Infrastructure.Repositories
{
    public class MotoRepository : Repository<Moto>, IMotoRepository
    {
        public MotoRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Moto>> GetDisponiveisAsync() =>
            await _dbSet.Where(m => !m.Vendida).ToListAsync();
    }
}
