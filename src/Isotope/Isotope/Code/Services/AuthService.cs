using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Isotope.Areas.Front.Dto;
using Isotope.Code.Utils;
using Isotope.Code.Utils.Helpers;
using Isotope.Data;
using Isotope.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Isotope.Code.Services
{
    /// <summary>
    /// Service for handling user authorization.
    /// </summary>
    public class AuthService
    {
        public AuthService(AppDbContext db, SignInManager<AppUser> signInMgr, UserManager<AppUser> userMgr)
        {
            _db = db;
            _signInMgr = signInMgr;
            _userMgr = userMgr;
        }

        private readonly AppDbContext _db;
        private readonly SignInManager<AppUser> _signInMgr;
        private readonly UserManager<AppUser> _userMgr;

        /// <summary>
        /// Authorizes the user.
        /// </summary>
        public async Task<LoginResponseVM> TryLoginAsync(LoginRequestVM request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.NormalizedUserName == request.Username.ToUpper());
            if(user == null)
                return new LoginResponseVM {ErrorMessage = "Log in failed"};
            
            var result = await _signInMgr.PasswordSignInAsync(user, request.Password, false, true);
            if (result.IsLockedOut)
                return new LoginResponseVM {ErrorMessage = "User locked out"};
            if(!result.Succeeded)
                return new LoginResponseVM {ErrorMessage = "Log in failed"};
            
            var roles = await _userMgr.GetRolesAsync(user);
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
        /// Generates the JWT token for an authorized user.
        /// </summary>
        private string GenerateToken(AppUser user, IEnumerable<string> roles)
        {
            var claims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.UserName));
            
            var token = new JwtSecurityToken(
                issuer: JwtHelper.Issuer,
                audience: JwtHelper.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(10),
                notBefore: DateTime.UtcNow,
                signingCredentials: JwtHelper.GetSigningCredentials()
            );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}