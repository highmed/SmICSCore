using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace SmICSCoreLib.Factories
{
    public class AQLQuery
    {
        private string Query { get; }
        public string Name { get; }

        public AQLQuery(string name, string query)
        {
            Name = name;
            Query = query;
        }
        
        public override string ToString()
        {
            return Query;
        }
    }
}
