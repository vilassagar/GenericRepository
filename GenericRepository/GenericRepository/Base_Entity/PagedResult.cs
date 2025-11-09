namespace GenericRepository.Base_Entity
{
    // Paged result class
    public class PagedResult<T>
    {
        public required IEnumerable<T> Items { get; init; }
        public required int TotalCount { get; init; }
        public required int PageNumber { get; init; }
        public required int PageSize { get; init; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;
    }
}
