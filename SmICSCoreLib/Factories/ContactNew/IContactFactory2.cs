using SmICSCoreLib.Factories.General;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.ContactNew
{
    public interface IContactFactory2
    {
        IAsyncEnumerable<ContactPatient> TestProcessAsync(Patient parameter);
    }
}