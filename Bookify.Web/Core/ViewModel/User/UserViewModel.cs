﻿namespace Bookify.Web.Core.ViewModel.User;


public class UserViewModel
{
    public string Id { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsDeleted { get; set; }
    public DateTime CreatedOn { get; set; }
    //public string? CreatedBy { get; set; } 
    public DateTime? LastUpdatedOn { get; set; }
}

