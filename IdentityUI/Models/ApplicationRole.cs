using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityUI.Models
{
    public class ApplicationRole : IRole<string>
    {
        public string Id { get; set; }       // Corresponds to the Role ID
        public string Name { get; set; }
        public ApplicationRole()
        {
            Id = Guid.NewGuid().ToString();
        }

    }
}