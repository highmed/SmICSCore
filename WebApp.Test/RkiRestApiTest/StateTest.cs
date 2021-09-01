using Newtonsoft.Json;
using SmICSCoreLib.StatistikDataModels;
using SmICSCoreLib.StatistikServices;
using System.IO;
using Xunit;

namespace WebApp.Test
{
    public class StateTest
    {
        RkiService rkiRestApi = new();

        [Fact]
        public void CkeckRespone()
        {
            State states = rkiRestApi.GetAllStates();
            Assert.NotNull(states);
        }

        [Fact]
        public void StatesNumber()
        {
            State states = rkiRestApi.GetAllStates();
            Assert.Equal(16, states.Features.Length);
        }


        [Fact]
        public void GetAState()
        {
            //string pathFile = @"../WebApp.Test/Resources";
            //State state = rkiRestApi.GetStateByName("Baden-Württemberg");
            //JSONWriter.Write(state, pathFile, "State");

            //expected
            State stateFromJson;
            string path = @"../../../../WebApp.Test/Resources/State.json";
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                stateFromJson = JsonConvert.DeserializeObject<State>(json);
            }

            //actual
            State state = rkiRestApi.GetStateByName("Baden-Württemberg");
               
            Assert.Equal(stateFromJson.Features[0].Attributes.Bundesland, state.Features[0].Attributes.Bundesland);
        }

      

        [Fact]
        public void GetStateDataTest()
        {
            StateData stateData = rkiRestApi.GetStateData(0);
            Assert.Equal(0, stateData.DataFeature[0].DataAttributes.BundeslandId);
        }

    }
}
