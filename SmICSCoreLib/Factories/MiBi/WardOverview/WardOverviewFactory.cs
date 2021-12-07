using Microsoft.Extensions.Logging;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.Lab.MibiLabData;
using SmICSCoreLib.Factories.PatientMovement;
using SmICSCoreLib.Factories.PatientMovement.ReceiveModels;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.Factories.MiBi.WardOverview
{
    public class WardOverviewFactory : IWardOverviewFactory
    {
        public IRestDataAccess RestDataAccess { get; private set; }
        private ILogger<WardOverviewFactory> _logger;
        public WardOverviewFactory(IRestDataAccess rest, ILogger<WardOverviewFactory> logger)
        {
            _logger = logger;
            RestDataAccess = rest;
        }

        public List<PatientLocation> Process(WardOverviewParameter parameter)
        {
            List<PatientLocation> patientLocations = RestDataAccess.AQLQuery<PatientLocation>(PatientsOnWardQuery(parameter));
            if (patientLocations != null)
            {
                return patientLocations;
            }
            return null;
        }

        /// <summary>
        /// Returns all patients on given ward where the admission is greater equal than the given start and less equal than the given end. 
        /// </summary>
        /// <param name="parameter"></param>
        private AQLQuery PatientsOnWardQuery(WardOverviewParameter parameter)
        {
            return new AQLQuery()
            {
                Name = "PatientsOnWard",
                Query = $@"SELECT e/ehr_status/subject/external_ref/id/value as PatientID,
                        i/items[at0001,'Zugehöriger Versorgungsfall (Kennung)']/value/value as CaseID,
                        u/data[at0001]/items[openEHR-EHR-CLUSTER.location.v1]/items[at0027]/value/value as Ward, 
                        u/data[at0001]/items[openEHR-EHR-CLUSTER.location.v1]/items[at0029]/value/value as Room
                        FROM EHR e 
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0] 
                        CONTAINS (CLUSTER u[openEHR-EHR-CLUSTER.case_identification.v0] 
                        AND ADMIN_ENTRY u[openEHR-EHR-ADMIN_ENTRY.hospitalization.v0] 
                        CONTAINS (CLUSTER a[openEHR-EHR-CLUSTER.location.v1]
                        AND CLUSTER o[openEHR-EHR-CLUSTER.organization.v0]))
                        WHERE c/name/value = 'Patientenaufenthalt' 
                        AND a/items[at0027]/value/value = '{parameter.Ward}' 
                        AND u/data[at0001]/items[at0004]/value/value >= '{ parameter.Start.ToString("yyyy-MM-dd") }'
                        AND u/data[at0001]/items[at0004]/value/value <= '{ parameter.End.ToString("yyyy-MM-dd") }'"
            };
        }
    }
}
