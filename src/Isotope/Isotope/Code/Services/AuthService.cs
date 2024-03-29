using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Isotope.Areas.Admin.Utils;
using Isotope.Areas.Front.Dto;
using Isotope.Areas.Front.Services;
using Isotope.Code.Utils.Helpers;
using Isotope.Data;
using Isotope.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Isotope.Code.Services;

/// <summary>
/// Service for handling user authorization.
/// </summary>
public class AuthService(AppDbContext db, SignInManager<AppUser> signInMgr, UserManager<AppUser> userMgr)
{
    /// <summary>
    /// Authorizes the user.
    /// </summary>
    public async Task<LoginResponseVM> TryLoginAsync(LoginRequestVM request)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.NormalizedUserName == request.Username.ToUpper());
        if (user == null)
            return new LoginResponseVM {ErrorMessage = "Log in failed"};

        var result = await signInMgr.PasswordSignInAsync(user, request.Password, false, true);
        if (result.IsLockedOut)
            return new LoginResponseVM {ErrorMessage = "User locked out"};
        if (!result.Succeeded)
            return new LoginResponseVM {ErrorMessage = "Log in failed"};

        var roles = await userMgr.GetRolesAsync(user);
        var token = GenerateToken(user, roles);
        return new LoginResponseVM
        {
            Success = true,
            User = new UserInfoVM
            {
                Username = user.UserName,
                IsAdmin = user.IsAdmin,
                Token = token
            }
        };
    }

    /// <summary>
    /// Checks if the token is valid.
    /// Returns the ClaimsPrincipal if everything is OK.
    /// </summary>
    public ClaimsPrincipal ValidateToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var args = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = JwtHelper.GetSigningCredentials().Key,
                ValidateIssuer = false,
                ValidateAudience = false
            };
            return handler.ValidateToken(token, args, out _);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Attempts to change the user's password.
    /// </summary>
    public async Task ChangePasswordAsync(ChangePasswordRequestVM request, UserContext userCtx)
    {
        if (request.NewPassword != request.NewPasswordRepeat)
            throw new OperationException("Passwords do not match!");

        if (!(request.NewPassword?.Length >= 6))
            throw new OperationException("Password must contain at least 6 characters!");

        var oldCheck = await userMgr.CheckPasswordAsync(userCtx.User, request.OldPassword);
        if (!oldCheck)
            throw new OperationException("Old password is invalid!");

        var result = await userMgr.ChangePasswordAsync(userCtx.User, request.OldPassword, request.NewPassword);
        if (!result.Succeeded)
            throw new OperationException("Failed to change password!");
    }

    #region Private helpers

    /// <summary>
    /// Generates the JWT token for an authorized user.
    /// </summary>
    private string GenerateToken(AppUser user, IEnumerable<string> roles)
    {
        var handler = new JwtSecurityTokenHandler();
        var claims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
        claims.Add(new Claim(ClaimTypes.Name, user.UserName!));

        var token = handler.CreateToken(
            new SecurityTokenDescriptor
            {
                Issuer = JwtHelper.Issuer,
                Audience = JwtHelper.Audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(10),
                SigningCredentials = JwtHelper.GetSigningCredentials()
            }
        );

        return handler.WriteToken(token);
    }

    #endregion
}