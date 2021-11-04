using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace SmICSCoreLib.REST
{
    public class openEHRJSONSerializer<T> where T : new()
    {
        public static List<T> ReceiveModelConstructor(HttpResponseMessage response)
        {
            if(response == null)
            {
                return null;
            }
            var str = response.Content.ReadAsStringAsync().Result;

            return Deserialize(str);
        }

        public static List<T> Deserialize(string json)
        {
            JObject obj = JsonConvert.DeserializeObject<JObject>(json, new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            });


            List<string> PropertyNames = GetPropertyNames((JArray)obj.Property("columns").Value);
            Dictionary<string, int> ParameterAllocation = GetParameterAllocation<T>(PropertyNames);

            if (obj.ContainsKey("rows") == false)
            {
                return null;
            }
            JArray Rows = (JArray)obj.Property("rows").Value;

            return CreateReceiveModelList(Rows, PropertyNames, ParameterAllocation);
        }
        private static List<string> GetPropertyNames(JArray columns) 
        {
            return columns.Children<JObject>().Select(x => x.Property("name").Value.ToString()).ToList<string>();
        }

        private static Dictionary<string, int> GetParameterAllocation<U>(List<string> PropertyNames)
        {
            Dictionary<string, int> paramteterAllocation = new Dictionary<string, int>();
            Type genericType = typeof(U);
            PropertyInfo[] properties = genericType.GetProperties();

            for(int i = 0; i < PropertyNames.Count; i++)
            {
                int propIndex = GetIndexOfProperty(properties, PropertyNames[i]);
                paramteterAllocation.Add(PropertyNames[i], propIndex);
            }

            return paramteterAllocation;
        }

        private static int GetIndexOfProperty(PropertyInfo[] Properties, string PropertyName)
        {
            return Properties.Select((prop, index) => new { prop, index }).First(x => x.prop.Name == PropertyName).index;
        }

        private static T CreateReceiveModel(JToken RowElement, List<string> PropertyNames, Dictionary<string, int> PropertyAllocation)
        {
            T receiveModel = new T();

            PropertyInfo[] properties = receiveModel.GetType().GetProperties();

            for (int i = 0; i < RowElement.Children().Count(); i++)
            {

                if(RowElement[i] == null || RowElement[i].ToString() == "")
                {
                    properties[PropertyAllocation[PropertyNames[i]]].SetValue(receiveModel, null);
                }
                else
                {
                    var conv = Convert.ChangeType(RowElement[i], properties[PropertyAllocation[PropertyNames[i]]].PropertyType);
                    properties[PropertyAllocation[PropertyNames[i]]].SetValue(receiveModel, conv);
                }
                
            }

            return receiveModel;
        }

        private static List<T> CreateReceiveModelList(JArray Rows, List<string> PropertyNames, Dictionary<string, int> PropertyAllocation)
        {
            List<T> ReceiveModelList = new List<T>();
            foreach(JToken RowElement in Rows)
            {
                ReceiveModelList.Add(CreateReceiveModel(RowElement, PropertyNames, PropertyAllocation));
            }
            return ReceiveModelList;
        }

    }
}
