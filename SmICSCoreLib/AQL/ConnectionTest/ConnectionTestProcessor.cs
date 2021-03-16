using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.ConnectionTest
{
    public class ConnectionTestProcessor
    {
        public static JArray Process(IDictionary parameter)
        {
            JArray result = null; // QueryHandler.SendQuery(AQLCatalog.ConnectionTest, null);

            if (result is null)
            {
                return new JArray();
            }

            return result;
        }
    }
}
