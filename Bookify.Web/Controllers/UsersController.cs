﻿namespace Bookify.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class UsersController : Controller
{
    // 24  , get way to open mvc from mobile;
    // allow to login by username and email;


    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
    }


    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var viewModel = _mapper.Map<IEnumerable<UserViewModel>>(users);
        return View(viewModel);
    }

    [HttpGet]
    [AjaxOnly]
    public async Task<IActionResult> Create()
    {
        var viewModel = new UserFormViewModel
        {
            Roles = await _roleManager.Roles
                            .Select(r => new SelectListItem
                            {
                                Text = r.Name,
                                Value = r.Name
                            })
                            .ToListAsync()
        };

        return PartialView("_Form", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserFormViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        ApplicationUser user = new()
        {
            FullName = model.FullName,
            UserName = model.UserName,
            Email = model.Email,
            CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRolesAsync(user, model.SelectedRoles);

            var viewModel = _mapper.Map<UserViewModel>(user);
            return PartialView("_UserRow", viewModel);
        }

        return BadRequest(string.Join(',', result.Errors.Select(e => e.Description)));
    }

    [HttpGet]
    [AjaxOnly]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return NotFound();

        var viewModel = _mapper.Map<UserFormViewModel>(user);

        viewModel.SelectedRoles = await _userManager.GetRolesAsync(user);
        viewModel.Roles = await _roleManager.Roles
                            .Select(r => new SelectListItem
                            {
                                Text = r.Name,
                                Value = r.Name
                            })
                            .ToListAsync();

        return PartialView("_Form", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserFormViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var user = await _userManager.FindByIdAsync(model.Id);

        if (user is null)
            return NotFound();

        user = _mapper.Map(model, user);
        user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        user.LastUpdatedOn = DateTime.Now;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);

            var rolesUpdated = !currentRoles.SequenceEqual(model.SelectedRoles);

            if (rolesUpdated)
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRolesAsync(user, model.SelectedRoles);
            }

            var viewModel = _mapper.Map<UserViewModel>(user);
            return PartialView("_UserRow", viewModel);
        }

        return BadRequest(string.Join(',', result.Errors.Select(e => e.Description)));
    }


    [HttpGet]
    [AjaxOnly]
    public async Task<IActionResult> ResetPassword(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return NotFound();

        var viewModel = new ResetPasswordFormViewModel { Id = user.Id };

        return PartialView("_ResetPasswordForm", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordFormViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var user = await _userManager.FindByIdAsync(model.Id);

        if (user is null)
            return NotFound();

        var currentPasswordHash = user.PasswordHash;

        await _userManager.RemovePasswordAsync(user);

        var result = await _userManager.AddPasswordAsync(user, model.Password);

        if (result.Succeeded)
        {
            user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            user.LastUpdatedOn = DateTime.Now;

            await _userManager.UpdateAsync(user);

            var viewModel = _mapper.Map<UserViewModel>(user);
            return PartialView("_UserRow", viewModel);
        }

        user.PasswordHash = currentPasswordHash;
        await _userManager.UpdateAsync(user);

        return BadRequest(string.Join(',', result.Errors.Select(e => e.Description)));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return NotFound();

        user.IsDeleted = !user.IsDeleted;
        user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        user.LastUpdatedOn = DateTime.Now;

        await _userManager.UpdateAsync(user);

        return Ok(user.LastUpdatedOn.ToString());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unlock(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return NotFound();

        var isLocked = await _userManager.IsLockedOutAsync(user);

        if (isLocked)
            await _userManager.SetLockoutEndDateAsync(user, null);

        return Ok();
    }



    public async Task<IActionResult> AllowUserName(UserFormViewModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);
        var isAllowed = user is null || user.Id.Equals(model.Id);

        return Json(isAllowed);
    }

    public async Task<IActionResult> AllowEmail(UserFormViewModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        var isAllowed = user is null || user.Id.Equals(model.Id);

        return Json(isAllowed);
    }
}