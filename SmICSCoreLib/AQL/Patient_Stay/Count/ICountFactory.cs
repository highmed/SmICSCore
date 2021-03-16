using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Patient_Stay.Count
{
    public interface ICountFactory
    {
       List<CountDataModel> Process(string nachweis);
    }
}
