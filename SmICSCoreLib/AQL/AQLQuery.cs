using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace SmICSCoreLib.AQL
{
    public class AQLQuery
    {
        private string query = "";
        public string Query { get { return this.query; } }

        public AQLQuery(string query)
        {   
            this.query = query;
        }
        
        public override string ToString()
        {
            return this.Query;
        }
    }
}
