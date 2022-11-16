using SmICSCoreLib.REST;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.MiBi
{
    public class AntibiogramFactory : IAntibiogramFactory
    {
        IRestDataAccess _rest;

        public AntibiogramFactory(IRestDataAccess rest)
        {
            _rest = rest;
        }

        public async Task<List<Antibiogram>> ProcessAsync(AntibiogramParameter parameters)
        {
            try
            {
                List<Antibiogram> antibiograms = await _rest.AQLQueryAsync<Antibiogram>(AntibiogramFromPathogen(parameters));
                if (antibiograms == null)
                {
                    return null;
                }
                return antibiograms;
            }
            catch
            {
                throw;
            }
            
        }

        private AQLQuery AntibiogramFromPathogen(AntibiogramParameter parameter)
        {
            return new AQLQuery("AntibiogramFromPathogen", @$"SELECT DISTINCT b/items[at0024]/value/value as Antibiotic,
                                                            b/items[at0024]/value/defining_code/code_string as AntibioticID,
                                                            b/items[at0004]/value/defining_code/code_string as Resistance,
                                                            b/items[at0001]/value/magnitude as MinInhibitorConcentration,
                                                            b/items[at0001]/value/units as MICUnit
                                                            FROM EHR e
                                                            CONTAINS COMPOSITION c
                                                            contains 
                                                                (CLUSTER m[openEHR-EHR-CLUSTER.case_identification.v0] 
                                                                and OBSERVATION j[openEHR-EHR-OBSERVATION.laboratory_test_result.v1] 
                                                                    CONTAINS CLUSTER w[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] 
                                                                    CONTAINS CLUSTER z[openEHR-EHR-CLUSTER.erregerdetails.v1] 
                                                                    CONTAINS CLUSTER u[openEHR-EHR-CLUSTER.laboratory_test_panel.v0] 
                                                                    CONTAINS CLUSTER b[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]) 
                                                            where w/items[at0001]/name='Erregername' 
                                                            and b/items[at0024]/name='Antibiotikum'   
                                                            and c/uid/value = '{ parameter.UID }' 
                                                            and w/items[at0001]/value/value = '{ parameter.Pathogen }' 
                                                            and w/items[at0027]/value/magnitude = '{ parameter.IsolatNo }'                                       
                                                            order by b/items[at0024]/value/value asc");
        }
    }
}
