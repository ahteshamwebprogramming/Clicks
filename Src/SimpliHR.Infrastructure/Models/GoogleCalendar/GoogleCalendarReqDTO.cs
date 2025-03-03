using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliHR.Infrastructure.Models.GoogleCalendar
{
    public partial class GoogleCalendarReqDTO
    {
        public string? Summary
        {
            get;
            set;
        }

        public string? Description
        {
            get;
            set;
        }
        public string? Organizer
        {
            get;
            set;
        }


        public string? StartTime
        {
            get;
            set;
        }

        public string? EndTime
        {
            get;
            set;
        }
        public DateTime? StartDateTime
        {
            get;
            set;
        }

        public DateTime? EndDateTime
        {
            get;
            set;
        }
        public string? Link
        {
            get;
            set;
        }
        public string? Message
        {
            get;
            set;
        }

    }
}
