using System;

namespace SmICSWebApp.Data.ContactComparison
{
    public class ContactPoint
    {
        public string Ward { get; internal set; }
        public string Room { get; internal set; }
        public string Departement { get; internal set; }
        public DateTime Start { get; internal set; }
        public DateTime? End { get; internal set; }
    }
}