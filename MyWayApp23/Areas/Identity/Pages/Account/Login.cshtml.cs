using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyWayApp23.Areas.Identity.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public LoginModel(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;

    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();
    public string ReturnUrl { get; set; } = string.Empty;

    public void OnGet()
    {
        ReturnUrl = Url.Content("~/");
    }
    public async Task<IActionResult> OnPostAsync()
    {
        ReturnUrl = Url.Content("~/");
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(Input.Username,
                Input.Password, true, lockoutOnFailure: false);
            if (result.Succeeded) return LocalRedirect(ReturnUrl);
        }

        return Page();
    }

    public class InputModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}