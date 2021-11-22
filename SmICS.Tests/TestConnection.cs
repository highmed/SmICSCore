using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.REST;
using System;

namespace SmICSDataGenerator.Tests
{
    public class TestConnection
    {
        public static RestDataAccess Initialize()
        {
            //OpenehrConfig.openehrEndpoint = Environment.GetEnvironmentVariable("OPENEHR_DB");
            //OpenehrConfig.openehrUser = Environment.GetEnvironmentVariable("OPENEHR_USER");
            //OpenehrConfig.openehrPassword = Environment.GetEnvironmentVariable("OPENEHR_PASSWD");

            //openehrconfig.openehrendpoint = "http://localhost:8080/ehrbase/rest/openehr/v1";
            //openehrconfig.openehrendpoint = "https://plri-highmed01.mh-hannover.local:8083/rest/openehr/v1";
            //openehrconfig.openehruser = "etltestuser";
            //openehrconfig.openehrpassword = "etltestuser#01";

            //OpenehrConfig.openehrEndpoint = "http://localhost:8080/ehrbase/rest/openehr/v1";
            OpenehrConfig.openehrEndpoint = "https://plri-highmed01.mh-hannover.local:8083/rest/openehr/v1";
            OpenehrConfig.openehrUser = "etltestuser";
            OpenehrConfig.openehrPassword = "etltestuser#01";

            RestClientConnector restClient = new RestClientConnector();
            return new RestDataAccess(NullLogger<RestDataAccess>.Instance, restClient);
        }
    }
}
