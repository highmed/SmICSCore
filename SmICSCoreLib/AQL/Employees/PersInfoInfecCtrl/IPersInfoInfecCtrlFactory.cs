using SmICSCoreLib.AQL.General;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.Employees.PersInfoInfecCtrl
{
    public interface IPersInfoInfecCtrlFactory
    {
        List<PersInfoInfecCtrlModel> Process(PatientListParameter parameter);
    }
}