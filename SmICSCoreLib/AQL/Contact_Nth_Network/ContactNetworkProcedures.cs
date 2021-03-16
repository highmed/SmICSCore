using Newtonsoft.Json.Linq;
using SmICSCoreLib.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Contact_Nth_Network
{
    public class ContactNetworkProcedures : IContactNetworkProcedures
    {
        private IContactNetworkFactory _contactNetFac;

        public ContactNetworkProcedures(IContactNetworkFactory contactNetFac)
        {
            _contactNetFac = contactNetFac;
        }
        public List<ContactModel> Contact_1stDegree_TTP(ContactParameter parameter)
        {
            return Contact_NthDegree_TTP_Degree(parameter);
        }
        public List<ContactModel> Contact_NthDegree_TTP_Degree(ContactParameter parameter)
        {
            return _contactNetFac.Process(parameter);
        }
    }
}
