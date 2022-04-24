using System;
using System.Collections.Generic;
using Application.Profiles;

namespace Application.Activities
{
    //When you want a list of activities, this class ActivityDto is used to get all the info
    public class ActivityDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string City { get; set; }

        public string Venue { get; set; }
        //Tells which username is the host
        public string HostUsername { get; set; }
        public bool IsCancelled { get; set; }
        //returns attendee Dto, which contains information about an attendee profile
        //This is mapped n Mapping profiles, to map ActivityAttendee and AttendeeDto
        public ICollection<AttendeeDto> Attendees { get; set; }
    }
}