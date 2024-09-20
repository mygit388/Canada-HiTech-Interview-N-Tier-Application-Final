using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace IdentityUI.Models
{
    public class ApplicationUser : IUser<string>
    {

        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public bool EmailConfirmed { get; set; }

        public ApplicationUser()
        {
            Id = Guid.NewGuid().ToString();
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            /* var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
             // Add custom user claims here
             return userIdentity;*/
            // Create a ClaimsIdentity for the user
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom claims here
            // Example: Adding a custom role claim
            /* userIdentity.AddClaim(new Claim(ClaimTypes.Name, this.UserName));
             userIdentity.AddClaim(new Claim(ClaimTypes.Email, this.Email));
             userIdentity.AddClaim(new Claim(ClaimTypes.Role, "User"));*/
            //  await userManager.AddClaimAsync(user.Id, new Claim(ClaimTypes.Name, user.UserName));
            //await userManager.AddClaimAsync(user.Id, new Claim(ClaimTypes.Email, user.Email));
            //await userManager.AddClaimAsync(user.Id, new Claim(ClaimTypes.Role, "User"));


            return userIdentity;
        }

    }
}