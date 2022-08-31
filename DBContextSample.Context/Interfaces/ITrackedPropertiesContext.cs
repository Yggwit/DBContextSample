namespace DBContextSample.Context.Interfaces
{
    public interface ITrackedPropertiesContext
    {
        internal List<string> TrackedProperties { get; set; }
        internal Dictionary<string, List<string>> TrackedPropertyValues { get; set; }
    }
}
