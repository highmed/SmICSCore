using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.Contact
{
    public interface IContactFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        List<Contact> Process(ContactParameter parameter);
    }
}