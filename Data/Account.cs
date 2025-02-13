namespace EFCore.Data
{
    public class Account
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}
