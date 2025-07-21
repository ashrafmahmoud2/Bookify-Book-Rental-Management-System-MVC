namespace Bookify.Web.Core.ViewModel.Serach;

public class SearchBookViewModel
{
    public string Value { get; set; } = null!;

    public string? AuthorName { get; set; }

    public string? Title { get; set; }

    public int? Id { get; set; }

    public string? ImageThumbnailUrl { get; set; }
}