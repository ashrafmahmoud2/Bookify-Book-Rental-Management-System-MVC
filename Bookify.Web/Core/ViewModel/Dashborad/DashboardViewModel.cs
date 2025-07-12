namespace Bookify.Web.Core.ViewModel.Dashborad;

public class DashboardViewModel
{
    public int NumberOfCopies { get; set; } 

    public int NumberOfSubscribers { get; set; }

    public IEnumerable<BookViewModel> LastAddedBooks { get; set; } = new List<BookViewModel>();
    public IEnumerable<BookViewModel> TopBooks { get; set; } = new List<BookViewModel>();
}
