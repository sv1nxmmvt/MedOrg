namespace MedOrg.Data.DTOs
{
    public class QueryResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class QueryResult<T> : QueryResult
    {
        public T? Data { get; set; }
        public int TotalCount { get; set; }
    }
}