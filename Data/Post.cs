
using Microsoft.EntityFrameworkCore;

namespace EFCore.Data
{
    [Index(nameof(Title), Name = "Title_Index")]
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }

      
    } 
}
