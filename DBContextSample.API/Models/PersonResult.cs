namespace DBContextSample.API.Models
{
    public class PersonResult : IFilterResult
    {
        public int? Id { get; set; }
        public Guid? Guid { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
