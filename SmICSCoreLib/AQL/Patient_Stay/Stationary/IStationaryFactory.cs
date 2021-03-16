using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Patient_Stay.Stationary
{
    public interface IStationaryFactory
    {
        List<StationaryDataModel> Process(string patientId, DateTime datum);

        //Wenn die Fallkennung vorhanden ist
        //List<StationaryDataModel> Process(string patientId, DateTime datum, string fallkenung);
    }
}
