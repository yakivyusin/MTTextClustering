namespace MTTextClustering.Models
{
    public record Text(
        Guid Id,
        string Content)
    {
        private readonly Lazy<string[]> _terms = new(() => Content.Split(
            new[] { "\r\n", "\r", "\n", " " },
            StringSplitOptions.RemoveEmptyEntries));

        public string[] Terms => _terms.Value;
    }
}
