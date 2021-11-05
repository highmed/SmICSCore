using System;

namespace SmICSWebApp.Data.OutbreakDetection
{
    public class OutbreakSavingInTimespan : OutbreakSaving
    {
        public DateTime Starttime { get; set; }
        public DateTime Endtime { get; set; }

    }
}