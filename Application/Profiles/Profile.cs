using System.Collections.Generic;
using Domain;

namespace Application.Profiles
{
    //used for use profile and attendee info
    public class Profile{
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
        public ICollection<Photo> Photos { get; set; }
    }
}