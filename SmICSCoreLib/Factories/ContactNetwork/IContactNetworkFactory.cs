using System.Collections.Generic;

namespace SmICSCoreLib.Factories.Contact_Nth_Network
{
    public interface IContactNetworkFactory
    {
        ContactModel Process(ContactParameter parameter);
    }
}