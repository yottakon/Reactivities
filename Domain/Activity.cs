using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    public class Activity
    {
        //Dot net recognizes Id as the primary key
        public Guid Id { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string City { get; set; }

        public string Venue { get; set; }
        public bool IsCancelled { get; set; }
        //Creating a new list prevents a null reference when you add something to the collection. You can't add something to null
        public ICollection<ActivityAttendee> Attendees { get; set; } = new List<ActivityAttendee>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}