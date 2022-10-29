using System.Security.Principal;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationServer.Account;

[SecurityHeaders]
[AllowAnonymous]
public class AccountController : Controller
{
    private readonly IClientStore _clientStore;
    private readonly IEventService _events;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly TestUserStore _users;

    public AccountController(
        TestUserStore users,
        IIdentityServerInteractionService interaction,
        IClientStore clientStore,
        IAuthenticationSchemeProvider schemeProvider,
        IEventService events)
    {
        _users = users;
        _interaction = interaction;
        _clientStore = clientStore;
        _schemeProvider = schemeProvider;
        _events = events;
    }

    [HttpGet]
    public async Task<IActionResult> Login(string returnUrl)
    {
        var vm = await BuildLoginViewModelAsync(returnUrl);
        // if (vm.IsExternalLoginOnly) return RedirectToAction("Challenge", "External", new {provider = vm.ExternalLoginScheme, returnUrl});

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string button)
    {
        var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

        if (ModelState.IsValid)
        {
            if (_users.ValidateCredentials(model.Username, model.Password))
            {
                var user = _users.FindByUsername(model.Username);
                await _events.RaiseAsync(new UserLoginSuccessEvent(user.Username, user.SubjectId, user.Username));

                var props = model.RememberLogin
                    ? new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
                    }
                    : null;

                var identity = new GenericIdentity(model.Username);
                var principal = new GenericPrincipal(identity, new string[0]);
                HttpContext.User = principal;
                
                await HttpContext.SignInAsync(new IdentityServerUser(user.SubjectId)
                {
                    DisplayName = user.Username,
                }, props);

                if (context is not null)
                {
                    if (await _clientStore.IsPkceClientAsync(context.Client.ClientId))
                    {
                        return View("Redirect", new RedirectViewModel { RedirectUrl = model.ReturnUrl });
                    }

                    return Redirect(model.ReturnUrl);
                }

                if (Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                if (string.IsNullOrEmpty(model.ReturnUrl))
                {
                    return Redirect("~/");
                }

                throw new Exception("invalid return URL");
            }

            await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials"));
            ModelState.AddModelError("", "Invalid username or password");
        }

        var vm = await BuildLoginViewModelAsync(model);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return View("LoggedOut");
    }


    private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
    {
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        if (context?.IdP != null)
            return new LoginViewModel
            {
                Parameters = context.Parameters,
                EnableLocalLogin = false,
                ReturnUrl = returnUrl,
                Username = context.LoginHint
                // ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } }
            };

        var schemes = await _schemeProvider.GetAllSchemesAsync();

        // missing code.

        return new LoginViewModel
        {
            Parameters = context?.Parameters,
            ReturnUrl = returnUrl,
            Username = context?.LoginHint
            // ExternalProviders = providers.ToArray()
        };
    }

    private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
    {
        var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
        vm.Username = model.Username;
        vm.RememberLogin = model.RememberLogin;
        return vm;
    }
}