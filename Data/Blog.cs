using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.Data
{
    public class Blog
    {
        public int Id { get; set; }
        public string Url { get; set; }  

       
    }
}
