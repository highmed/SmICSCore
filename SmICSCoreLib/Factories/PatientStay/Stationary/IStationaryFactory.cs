using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Factories.PatientStay.Stationary
{
    public interface IStationaryFactory
    {
        List<StationaryDataModel> Process(string patientId, string fallkennung, DateTime datum);

        List<StationaryDataModel> ProcessFromCase(string patientId, string fallId);
        
        List<StationaryDataModel> ProcessFromDate(DateTime datum);

    }
}
