using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.ConnectionTest
{
    public class ConnectionTest : IConnectionTest
    {
        public JArray Test()
        {
            return null; // DataProcessHandler.ProcessResponse(ConnectionTestProcessor.Process, null);
        }
    }
}
