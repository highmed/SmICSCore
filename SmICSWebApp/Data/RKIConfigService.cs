using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmICSCoreLib.REST;
using SmICSCoreLib.AQL.RKIConfig;
using System.Threading;

namespace SmICSWebApp.Data
{
    public class RKIConfigService
    {
        private IRestDataAccess _restData;

        public RKIConfigService(IRestDataAccess restData)
        {
            _restData = restData;
        }

        //public Task<IEnumerable<RKIConfigTemplate>> GetStationAsync(CancellationToken ct = default)
        //{
        //    return;
        //}
    }
}
