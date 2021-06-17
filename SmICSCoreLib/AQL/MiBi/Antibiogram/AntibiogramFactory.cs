using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.MiBi
{
    public class AntibiogramFactory : IAntibiogramFactory
    {
        IRestDataAccess _rest;

        public AntibiogramFactory(IRestDataAccess rest)
        {
            _rest = rest;
        }

        public List<Antibiogram> Process(AntibiogramParameter parameters)
        {
            List<Antibiogram> antibiograms = _rest.AQLQuery<Antibiogram>(AQLCatalog.AntibiogramFromPathogen(parameters));
            if (antibiograms == null)
            {
                return null;
            }
            return antibiograms;
        }
    }
}
