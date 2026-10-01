namespace AutomoveisVendasApi.Application.Common
{
    /// <summary>Envelope paginado devolvido pela listagem v2.</summary>
    public class PagedResult<T>
    {
        public long Page { get; init; }
        public int PageSize { get; init; }
        public int TotalItems { get; init; }
        public int TotalPages { get; init; }
        public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;

        public static PagedResult<T> Create(IReadOnlyList<T> items, long page, int pageSize, int totalItems) => new()
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            // teto de totalItems / pageSize (pageSize já foi validado: 1..100)
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            Items = items
        };

        public PagedResult<TOut> Map<TOut>(Func<T, TOut> selector) => new()
        {
            Page = Page,
            PageSize = PageSize,
            TotalItems = TotalItems,
            TotalPages = TotalPages,
            Items = Items.Select(selector).ToList()
        };
    }
}