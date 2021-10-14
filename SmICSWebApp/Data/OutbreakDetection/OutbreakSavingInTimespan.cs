using System;

namespace SmICSWebApp.Data.OutbreakDetection
{
    public class OutbreakSavingInTimespan : OutbreakSaving
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

    }
}