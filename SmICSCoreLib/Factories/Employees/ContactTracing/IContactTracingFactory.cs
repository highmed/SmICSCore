using SmICSCoreLib.Factories.General;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Employees.ContactTracing
{
    public interface IContactTracingFactory
    {
        List<ContactTracingReceiveModel> Process(PatientListParameter parameter);
    }
}