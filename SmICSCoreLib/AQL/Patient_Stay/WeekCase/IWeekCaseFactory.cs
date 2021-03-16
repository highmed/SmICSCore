using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Patient_Stay.WeekCase
{
    public interface IWeekCaseFactory
    {
        List<WeekCaseDataModel> Process(DateTime startDate, DateTime endDate);
    }
}
