using SmICSCoreLib.REST;

namespace SmICSCoreLib.Factories.ContactNetwork
{
    public interface IContactNetworkFactory
    {
        IRestDataAccess RestDataAccess { get; }
        ContactModel Process(ContactParameter parameter);
    }
}