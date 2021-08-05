using SmICSCoreLib.StatistikServices;
using System;
using Xunit;

namespace WebApp.Test
{
    public class SetCaseColorTest
    {
        RkiService rkiRestApi = new();

        [Fact]
        public void CkeckSetCaseColor()
        {
            string color = rkiRestApi.SetCaseColor("-10", "-5");

            Assert.Equal("#66C166", color);
        }

        [Fact]
        public void CkeckSetCaseColor2()
        {
            string color = rkiRestApi.SetCaseColor("100", "20");

            Assert.Equal("#F35C58", color);
        }

        [Fact]
        public void CkeckSetCaseColor3()
        {
            string color = rkiRestApi.SetCaseColor("20", "20");

            Assert.Equal("#FFC037", color);
        }

        [Fact]
        public void CkeckIncorrectInput()
        {
            string color = rkiRestApi.SetCaseColor("", "");

            Assert.Equal("#8CA2AE", color);
        }

    }
}
