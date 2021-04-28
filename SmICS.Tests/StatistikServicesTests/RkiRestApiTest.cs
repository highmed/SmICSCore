using SmICSCoreLib.StatistikDataModels;
using SmICSCoreLib.StatistikServices;
using Xunit;


namespace SmICSFactory.Tests.StatistikServicesTests
{
    public class RkiRestApiTest
    {
        [Fact]
        public void GetAllStatesTest() {
            RestClient client = new();
            RkiRestApi rkiRestApi = new ();

            State state = rkiRestApi.GetAllStates();

            Assert.NotNull(state);
    }
    }
}
