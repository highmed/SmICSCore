using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MiBi.PatientView
{
    public class SpecimenFactory : ISpecimenFactory
    {
        public IRestDataAccess _restDataAccess { get; set; }
        private readonly IPathogenFactory _pathogegFac;
        public SpecimenFactory(IRestDataAccess restDataAccess, IPathogenFactory pathogegFac)
        {
            _restDataAccess = restDataAccess;
            _pathogegFac = pathogegFac;
        }

        public List<Specimen> Process(SpecimenParameter specimenParameter, PathogenParameter pathogen = null)
        {
            List<Specimen> specimens = _restDataAccess.AQLQuery<Specimen>(SpecimenQuery(specimenParameter));
            foreach (Specimen specimen in specimens)
            {
                PathogenParameter parameter;
                if (pathogen == null)
                {
                    parameter = specimenParameter as PathogenParameter;
                }
                else
                {
                    parameter = new PathogenParameter(pathogen);
                    parameter.Name = pathogen.Name;
                }
                parameter.LabID = specimen.LabID;

                specimen.Pathogens = _pathogegFac.Process(parameter);
            }
            return specimens;
        }

        private AQLQuery SpecimenQuery(SpecimenParameter parameter)
        {
            return new AQLQuery()
            {
                Name = "Specimen - Mikrobiologischer Befund",
                Query = @$"SELECT a/items[at0029]/value as Kind,
                        a/items[at0001]/value as LabID,
                        a/items[at0034]/value as SpecimenReceiptDate,
                        o/items[at0001]/value as Location,
                        a/items[at0015]/value as SpecimenCollectionDateTime,
                        FROM EHR e
                        CONTAINS COMPOSITION c
                        CONTAINS CLUSTER a[openEHR-EHR-CLUSTER.specimen.v1] 
                        CONTAINS (CLUSTER o[openEHR-EHR-CLUSTER.anatomical_location.v1]) 
                        WHERE c/name/value='Mikrobiologischer Befund'
                        AND c/uid/value='{parameter.UID}'"
            };
        }
    }
}
