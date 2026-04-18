using AutomoveisVendasApi.Application.Interfaces;
using AutomoveisVendasApi.Domain.Entities;
using AutomoveisVendasApi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AutomoveisVendasApi.Infrastructure.Repositories
{
    public class ClienteRepository : Repository<Cliente>, IClienteRepository
    {
        public ClienteRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Cliente?> GetByEmailAsync(string email) =>
            await _dbSet.FirstOrDefaultAsync(c => c.Email == email);
    }
}
