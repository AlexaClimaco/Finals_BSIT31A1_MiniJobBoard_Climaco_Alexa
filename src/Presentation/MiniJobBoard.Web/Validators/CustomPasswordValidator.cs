using Microsoft.AspNetCore.Identity;
using MiniJobBoard.Infrastructure.Entities;

namespace MiniJobBoard.Web.Validators;

public class CustomPasswordValidator : IPasswordValidator<ApplicationUser>
{
    public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return Task.FromResult(IdentityResult.Failed(new IdentityError
            {
                Code = "PasswordEmpty",
                Description = "Password cannot be empty."
            }));
        }

        var errors = new List<IdentityError>();

        // Check for at least 2 uppercase letters
        var upperCaseCount = password.Count(char.IsUpper);
        if (upperCaseCount < 2)
        {
            errors.Add(new IdentityError
            {
                Code = "PasswordUppercase",
                Description = "Password must contain at least 2 uppercase letters."
            });
        }

        // Check for at least 3 numbers
        var digitCount = password.Count(char.IsDigit);
        if (digitCount < 3)
        {
            errors.Add(new IdentityError
            {
                Code = "PasswordDigit",
                Description = "Password must contain at least 3 numbers."
            });
        }

        // Check for at least 3 symbols
        var symbolCount = password.Count(c => !char.IsLetterOrDigit(c));
        if (symbolCount < 3)
        {
            errors.Add(new IdentityError
            {
                Code = "PasswordSymbol",
                Description = "Password must contain at least 3 symbols."
            });
        }

        if (errors.Count > 0)
        {
            return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
        }

        return Task.FromResult(IdentityResult.Success);
    }
}

