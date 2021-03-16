using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.Contact_Nth_Network
{
    public interface IContactNetworkProcedures
    {
        List<ContactModel> Contact_1stDegree_TTP(ContactParameter parameter);
        List<ContactModel> Contact_NthDegree_TTP_Degree(ContactParameter parameter);
    }
}