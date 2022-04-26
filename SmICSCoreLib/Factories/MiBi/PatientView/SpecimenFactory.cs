using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.REST;
using System;
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
            if(specimens is not null)
            {
                foreach (Specimen specimen in specimens)
                {
                    if (pathogen == null)
                    {
                        pathogen = new PathogenParameter(specimenParameter);
                    }
                    else
                    {
                        pathogen.UID = specimenParameter.UID;
                    }
                    pathogen.LabID = specimen.LabID;

                    specimen.Pathogens = _pathogegFac.Process(pathogen);
                }
            }
            return specimens;
        }

        private AQLQuery SpecimenQuery(SpecimenParameter parameter)
        {
            return new AQLQuery()
            {
                Name = "Specimen",
                /* Eigentliche Query mit Location die eingefügt werden muss. Zu Testzwecken aber für Hannover rausgenommen
                Query = @$"SELECT a/items[at0029]/value/value as Kind,
                       a/items[at0029]/value/defining_code/code_string as KindCode,
                       a/items[at0001]/value/id as LabID,
                       a/items[at0034]/value/value as SpecimenReceiptDate,
                       o/items[at0001]/value/value as Location,
                       a/items[at0015]/value/value as SpecimenCollectionDateTime
                        FROM EHR e
                        CONTAINS COMPOSITION c
                        CONTAINS CLUSTER a[openEHR-EHR-CLUSTER.specimen.v1] 
                        CONTAINS (CLUSTER o[openEHR-EHR-CLUSTER.anatomical_location.v1]) 
                        WHERE c/uid/value='{parameter.UID}'
                        ORDER BY a/items[at0034]/value/value ASC"*/
                Query = @$"SELECT a/items[at0029]/value/value as Kind,
                       a/items[at0029]/value/defining_code/code_string as KindCode,
                       a/items[at0001]/value/id as LabID,
                       a/items[at0034]/value/value as SpecimenReceiptDate,
                       'Abnahmestelle' as Location,
                       a/items[at0015]/value/value as SpecimenCollectionDateTime
                        FROM EHR e
                        CONTAINS COMPOSITION c
                        CONTAINS CLUSTER a[openEHR-EHR-CLUSTER.specimen.v1] 
                        WHERE c/uid/value='{parameter.UID}'
                        ORDER BY a/items[at0034]/value/value ASC"
            };
        }
    }
}
