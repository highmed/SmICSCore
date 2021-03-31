using System.Collections.Generic;

namespace SmICSCoreLib.AQL.Contact_Nth_Network
{
    public interface IContactNetworkFactory
    {
        ContactModel Process(ContactParameter parameter);
    }
}