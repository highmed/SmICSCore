using SmICSCoreLib.Factories.General;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Employees.PersInfoInfecCtrl
{
    public interface IPersInfoInfecCtrlFactory
    {
        List<PersInfoInfecCtrlModel> Process(PatientListParameter parameter);
    }
}