using AutomoveisVendasApi.Domain.Exceptions;

namespace AutomoveisVendasApi.Application.Common
{
    public sealed record PageRequest
    {
        public const int DefaultPage = 1;
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 100;

        public long Page { get; }
        public int PageSize { get; }

        public long Offset => (Page - 1) > long.MaxValue / PageSize
            ? long.MaxValue
            : (Page - 1) * PageSize;

        private PageRequest(long page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }

        public static PageRequest Create(long page, int pageSize)
        {
            if (page < 1)
                throw new DomainException($"O parâmetro 'page' deve ser um inteiro maior ou igual a 1. Valor recebido: {page}.");

            if (pageSize < 1 || pageSize > MaxPageSize)
                throw new DomainException($"O parâmetro 'pageSize' deve estar entre 1 e {MaxPageSize}. Valor recebido: {pageSize}.");

            return new PageRequest(page, pageSize);
        }
    }
}
