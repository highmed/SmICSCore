using System;
using System.Collections.Generic;

namespace SmICSWebApp.Data.Contact
{
    public class FilterParameter
    {
        public string ResistenceDetail { get; set; }
        public string ContactDetail { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<string> ResistanceOptions { get; internal set; } = new List<string> { "Alles" };
        public List<string> ContactOptions { get; } = new List<string> { "Alles", "Stationskontakte", "Zimmerkontakte" };

    }
}
