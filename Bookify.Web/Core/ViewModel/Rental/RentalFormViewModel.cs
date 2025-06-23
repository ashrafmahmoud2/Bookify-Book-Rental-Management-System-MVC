﻿namespace Bookify.Web.Core.ViewModel.Rental;

public class RentalFormViewModel
{
    public string SubscriberKey { get; set; } = null!;

    public IList<int> SelectedCopies { get; set; } = new List<int>();

    public int? MaxAllowedCopies { get; set; }

}


