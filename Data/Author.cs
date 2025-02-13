namespace EFCore.Data
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Account Account { get; set; }
    }
}
