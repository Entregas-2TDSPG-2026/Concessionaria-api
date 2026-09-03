
using AutomoveisVendasApi.Application.DTOs;

namespace AutomoveisVendasApi.Application.Interfaces
{
   
    public interface IVendaService
    {
        Task<VendaDto> CriarVendaAsync(CreateVendaDto dto);
    }
}