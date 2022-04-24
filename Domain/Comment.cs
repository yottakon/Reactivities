using System;

namespace Domain
{
    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public AppUser Author { get; set; }
        public Activity Activity { get; set; }
        //set to DateTime.UtcNow, meaning it will be saved to the database as that timezone
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}