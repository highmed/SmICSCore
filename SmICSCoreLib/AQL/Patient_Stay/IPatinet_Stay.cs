using System;
using System.Collections.Generic;
using System.Text;
using SmICSCoreLib.AQL.Patient_Stay.Cases;
using SmICSCoreLib.AQL.Patient_Stay.Count;
using SmICSCoreLib.AQL.Patient_Stay.Stationary;
using SmICSCoreLib.AQL.Patient_Stay.WeekCase;

namespace SmICSCoreLib.AQL.Patient_Stay
{
    public interface IPatinet_Stay
    {
        List<StationaryDataModel> Stationary_Stay(string patientId, string fallkennung, DateTime datum);
        List<CountDataModel> CovidPat(string nachweis);
        List<CaseDataModel> Case(DateTime date);
        List<WeekCaseDataModel> WeekCase(DateTime startDate, DateTime endDate);
        List<StationaryDataModel> StayFromCase(string patientId, string fallId);

    }
}
