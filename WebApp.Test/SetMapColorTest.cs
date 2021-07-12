using SmICSCoreLib.StatistikServices;
using Xunit;

namespace WebApp.Test
{
    public class SetMapColorTest
    {
        RkiRestApi rkiRestApi = new();

        [Fact]
        public void CkeckSetMapColor()
        {
            string color = rkiRestApi.SetMapColor("50");
            
            Assert.Equal("#D43624", color);
        }

        [Fact]
        public void CkeckIncorrectInput()
        {
            string color = rkiRestApi.SetMapColor("");

            Assert.Equal("#FFFFFF", color);
        }

    }
}
