using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain
{
    public class AppUser : IdentityUser
    {
        //IdentityUser has bunch of needed properties like ID
        //the two below are two new properties
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public ICollection<ActivityAttendee> Activities { get; set; }
        public ICollection<Photo> Photos { get; set; } 
    }
}