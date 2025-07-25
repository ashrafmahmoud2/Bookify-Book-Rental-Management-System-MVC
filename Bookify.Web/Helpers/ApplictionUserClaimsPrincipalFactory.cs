﻿using Microsoft.Extensions.Options;

namespace Bookify.Web.Helpers;

public class ApplictionUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    public ApplictionUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> options) : base(userManager, roleManager, options)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        // Add custom claims here if needed
        identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FullName ));
        return identity;
    }
}
