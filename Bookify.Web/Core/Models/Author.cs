namespace Bookify.Web.Core.Models;


[Index(nameof(Name), IsUnique = true)]
public class Author : BaseModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
