using SmICSCoreLib.DB;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MenuList
{
    public class MenuListFactory : IMenuListFactory
    {
        public IRestDataAccess RestDataAccess { get; set; }
        //private IDataAccess _db { get; set; }
        public MenuListFactory(IRestDataAccess restDataAccess)
        {
            RestDataAccess = restDataAccess;
           //_db = db;
        }

        public List<WardMenuEntry> Wards()
        {
            return RestDataAccess.AQLQuery<WardMenuEntry>(WardList());
        }

        public List<PathogenMenuEntry> Pathogens()
        {
            List<PathogenMenuEntry> entries = RestDataAccess.AQLQuery<PathogenMenuEntry>(MibiPathogenList());
            List<PathogenMenuEntry> viroentries = RestDataAccess.AQLQuery<PathogenMenuEntry>(ViroPathogenList());
            entries.AddRange(viroentries);
            //string sql = "";
            //_db.SaveData(sql, new { });
            return entries;
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

        private AQLQuery MibiPathogenList()
        {
            return new AQLQuery()
            {
                Name = "PathogenList",
                Query = @"SELECT DISTINCT w/items[at0001]/value/value as Name,
                        w/items[at0001]/value/defining_code/code_string as ID
                        FROM EHR e
                        CONTAINS COMPOSITION c
                        CONTAINS CLUSTER w[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]
                        WHERE c/name/value='Mikrobiologischer Befund' and w/items[at0001]/name/value='Erregername'"

            };
        } 
        private AQLQuery ViroPathogenList()
        {
            return new AQLQuery()
            {
                Name = "PathogenList",
                Query = @"SELECT DISTINCT w/items[at0024]/value/value as Name,
                        w/items[at0024]/value/defining_code/code_string as ID
                        FROM EHR e
                        CONTAINS COMPOSITION c
                        CONTAINS CLUSTER w[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]
                        WHERE c/name/value='Virologischer Befund'"

            };
        }
    }
}
