using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.Web.Core.ViewModel.Rental;

public class RentalReturnFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Penalty Paid")]
    [AssertThat("(TotleDelayInDays == 0 &&  PenaltyPaid == false) || PenaltyPaid == true", ErrorMessage = Errors.PanltyShoudPaid)]
    public bool PenaltyPaid { get; set; }

    public IList<RentalCopyViewModel> Copies { get; set; } = new List<RentalCopyViewModel>();  // we use list to using the index in the view , we don't use the Ilist besuce we don't want to edit it


    public List<ReturnCopyViewModel> SelectedCopies { get; set; } = new();

    public bool AllowExtend { get; set; }

    public int TotleDelayInDays
    {
        get
        {
            return Copies.Sum(c => c.DelayInDays);
        }
    }
}

public class ReturnCopyViewModel
{

    public int Id { get; set; }

    public bool? IsReturned { get; set; }

}








