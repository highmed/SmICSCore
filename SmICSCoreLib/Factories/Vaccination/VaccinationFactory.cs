﻿using Microsoft.Extensions.Logging;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.REST;
using System.Collections.Generic;


namespace SmICSCoreLib.Factories.Vaccination
{
    public class VaccinationFactory : IVaccinationFactory
    {
        public IRestDataAccess RestDataAccess { get; }
        private readonly ILogger<VaccinationFactory> _logger;
        public VaccinationFactory(IRestDataAccess restData, ILogger<VaccinationFactory> logger)
        {
            _logger = logger;
            RestDataAccess = restData;
        }
        public List<VaccinationModel> Process(PatientListParameter parameter)
        {

            List<VaccinationModel> vaccList = RestDataAccess.AQLQueryAsync<VaccinationModel>(AQLCatalog.PatientVaccination(parameter)).GetAwaiter().GetResult();

            if (vaccList is null)
            {
                return new List<VaccinationModel>();
            }

            return vaccList;
        }

        public List<VaccinationModel> ProcessSpecificVaccination(PatientListParameter parameter, string vaccination)
        {
            List<VaccinationModel> vaccList = RestDataAccess.AQLQueryAsync<VaccinationModel>(AQLCatalog.SpecificVaccination(parameter, vaccination)).GetAwaiter().GetResult();

            if (vaccList is null)
            {
                return new List<VaccinationModel>();
            }

            return vaccList;
        }
    }
}
