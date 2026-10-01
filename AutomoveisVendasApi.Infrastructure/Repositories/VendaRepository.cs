using AutomoveisVendasApi.Application.Common;
using AutomoveisVendasApi.Application.Interfaces;
using AutomoveisVendasApi.Domain.Entities;
using AutomoveisVendasApi.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AutomoveisVendasApi.Infrastructure.Repositories
{
    public class VendaRepository : Repository<Venda>, IVendaRepository
    {
        public VendaRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Venda>> GetByClienteIdAsync(int clienteId) =>
            await _dbSet
                .Where(v => v.ClienteId == clienteId)
                .Include(v => v.Pagamentos)
                .ToListAsync();

        public async Task<IEnumerable<Venda>> GetWithDetailsAsync() =>
            await _dbSet
                .Include(v => v.Cliente)
                .Include(v => v.Carro)
                .Include(v => v.Moto)
                .Include(v => v.Pagamentos)
                .ToListAsync();

        public async Task<PagedResult<Venda>> GetPagedAsync(PageRequest request)
        {
            // 1) COUNT no banco
            var totalItems = await _dbSet.CountAsync();

            // Página além do total: 200 com items vazio (nem precisa ir ao banco de novo).
            if (request.Offset >= totalItems)
                return PagedResult<Venda>.Create(Array.Empty<Venda>(), request.Page, request.PageSize, totalItems);

            // 2) ORDER BY estável + OFFSET/LIMIT no IQueryable, só então ToList.
            //    VendaId como desempate garante que página 1 e 2 nunca se sobreponham.
            var items = await _dbSet
                .AsNoTracking()
                .Include(v => v.Cliente)
                .Include(v => v.Carro)
                .Include(v => v.Moto)
                .Include(v => v.Pagamentos)
                .OrderByDescending(v => v.DataVenda)
                .ThenByDescending(v => v.VendaId)
                .Skip((int)request.Offset)   // seguro: Offset < totalItems (int)
                .Take(request.PageSize)
                .ToListAsync();

            return PagedResult<Venda>.Create(items, request.Page, request.PageSize, totalItems);
        }
    }
}