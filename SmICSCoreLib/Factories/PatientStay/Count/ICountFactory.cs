using SmICSCoreLib.Factories.General;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.Factories.PatientStay.Count
{
    public interface ICountFactory
    {
       List<CountDataModel> Process(string nachweis);

       List<CountDataModel> ProcessFromID(string nachweis, PatientListParameter parameter);
    }
}
