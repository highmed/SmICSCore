using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.REST;
using System;

namespace SmICSDataGenerator.Tests
{
    public class TestConnection
    {
        public static RestDataAccess Initialize()
        {
            //OpenehrConfig.openehrEndpoint = "http://localhost:8080/ehrbase/rest/openehr/v1";
            //OpenehrConfig.openehrEndpoint = Environment.GetEnvironmentVariable("OPENEHRDB");
            OpenehrConfig.openehrEndpoint = "https://plri-highmed01.mh-hannover.local:8083/rest/openehr/v1";
            OpenehrConfig.openehrUser = "etltestuser";
            OpenehrConfig.openehrPassword = "etltestuser#01";

            RestClientConnector restClient = new RestClientConnector();
            return new RestDataAccess(NullLogger<RestDataAccess>.Instance, restClient);
        }
    }
}
