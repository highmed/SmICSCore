using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MenuList
{
    public class MenuListFactory : IMenuListFactory
    {
        public IRestDataAccess RestDataAccess { get; set; }
        public MenuListFactory(IRestDataAccess restDataAccess)
        {
            RestDataAccess = restDataAccess;
        }

        public List<WardMenuEntry> Wards()
        {
            return RestDataAccess.AQLQuery<WardMenuEntry>(WardList());
        }

        public List<PathogenMenuEntry> Pathogens()
        {
            return RestDataAccess.AQLQuery<PathogenMenuEntry>(PathogenList());
        }

        private AQLQuery WardList()
        {
            return new AQLQuery()
            {
                Name = "WardList",
                Query = @"SELECT DISTINCT h/items[at0027]/value/value as ID
                        FROM EHR e
                        CONTAINS COMPOSITION c
                        CONTAINS CLUSTER h[openEHR-EHR-CLUSTER.location.v1]"
            };
        }

        private AQLQuery PathogenList()
        {
            return new AQLQuery()
            {
                Name = "PathogenList",
                Query = @"SELECT DISTINCT q/items[at0024,'Virusnachweistest']/value/value as Name
                        FROM EHR e
                        CONTAINS COMPOSITION c
                        CONTAINS CLUSTER q[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]"

            };
        }
    }
}
