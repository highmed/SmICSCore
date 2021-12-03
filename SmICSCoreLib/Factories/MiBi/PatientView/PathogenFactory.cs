using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public class PathogenFactory : IPathogenFactory
    {
        public IRestDataAccess RestDataAccess { get; set; }
        private readonly IAntibiogramFactory _antibiogramFac;
        public PathogenFactory(IRestDataAccess restDataAccess, IAntibiogramFactory antibiogramFac)
        {
            RestDataAccess = restDataAccess;
            _antibiogramFac = antibiogramFac;
        }

        public List<Pathogen> Process(PathogenParameter pathogenParameter)
        {
            List<Pathogen> pathogens = RestDataAccess.AQLQuery<Pathogen>(PathogenQuery(pathogenParameter));
            foreach (Pathogen pathogen in pathogens)
            {
                AntibiogramParameter parameter = pathogenParameter as AntibiogramParameter;
                parameter.IsolatNo = pathogen.IsolatNr;
                parameter.Pathogen = pathogen.Name;

                pathogen.Antibiograms = _antibiogramFac.Process(parameter);
            }
            return pathogens;
        }

        private AQLQuery PathogenQuery(PathogenParameter parameter)
        {
            return new AQLQuery()
            {
                Name = "Pathogen - Mikrobiologischer Befunde",
                Query = @$"SELECT u/items[at0001,'Erregername']/value/value as Name,
                       u/items[at0024,'Nachweis?']/value/value as Result,
                       b/items[at0003]/value as Rate,
                       u/items[at0027,'Isolatnummer']/value/magnitude as IsolatNr
                        FROM EHR e
                        CONTAINS COMPOSITION c
                        CONTAINS OBSERVATION s[openEHR-EHR-OBSERVATION.laboratory_test_result.v1]
                        CONTAINS (CLUSTER u[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1] 
                        CONTAINS (CLUSTER b[openEHR-EHR-CLUSTER.erregerdetails.v1])) 
                        WHERE c/name/value='Mikrobiologischer Befund'
                        AND c/uid/value='{parameter.UID}'
                        AND u/items[at0026,'Zugehörige Laborprobe']/value/id = '{parameter.LabID}'"
            };
        }
    }
}
