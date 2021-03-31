using Microsoft.Extensions.Logging;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.REST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmICSCoreLib.AQL.PatientInformation.Vaccination
{
    public class VaccinationFactory : IVaccinationFactory
    {
        private IRestDataAccess _restData;
        private readonly ILogger<VaccinationFactory> _logger;
        public VaccinationFactory(IRestDataAccess restData, ILogger<VaccinationFactory> logger)
        {
            _logger = logger;
            _restData = restData;
        }
        public List<VaccinationModel> Process(PatientListParameter parameter)
        {

            List<VaccinationModel> vaccList = _restData.AQLQuery<VaccinationModel>(AQLCatalog.PatientVaccination(parameter));

            if (vaccList is null)
            {
                return new List<VaccinationModel>();
            }

            return vaccList;
        }
    }
}
