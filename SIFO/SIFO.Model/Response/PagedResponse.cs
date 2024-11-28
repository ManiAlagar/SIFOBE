namespace SIFO.Model.Response
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Result { get; set; } = new List<T>();

        public long? TotalCount { get; set; } = 0;

        public long TotalPages { get; set; }

        public long CurrentPage { get; set; }
    }
}
