using Microsoft.Extensions.Logging;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.REST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmICSCoreLib.AQL.PatientInformation.Symptome
{
    public class SymptomFactory : ISymptomFactory
    {
        private IRestDataAccess _restData;
        private readonly ILogger<SymptomFactory> _logger;
        public SymptomFactory(IRestDataAccess restData, ILogger<SymptomFactory> logger)
        {
            _logger = logger;
            _restData = restData;
        }
        public List<SymptomModel> Process(PatientListParameter parameter)
        {
            List<SymptomModel> symptomList = new List<SymptomModel>();

            try
            {
                List<SymptomModel> symptomList_VS = _restData.AQLQuery<SymptomModel>(AQLCatalog.PatientSymptom_VS(parameter));

                if (symptomList_VS != null)
                {
                    symptomList = symptomList_VS;
                }

                List<SymptomModel> symptomList_AS = _restData.AQLQuery<SymptomModel>(AQLCatalog.PatientSymptom_AS(parameter));

                if (symptomList_AS != null)
                {
                    symptomList = symptomList.Concat(symptomList_AS).ToList();
                }

                List<SymptomModel> symptomList_US = _restData.AQLQuery<SymptomModel>(AQLCatalog.PatientSymptom_US(parameter));

                if (symptomList_US != null)
                {
                    symptomList = symptomList.Concat(symptomList_US).ToList();
                }

                _logger.LogDebug("Information found.");
            } 
            catch (Exception e)
            {
                _logger.LogError(e, "This Information could not be found.");
            } 
            finally
            {
                _logger.LogDebug("AQL query worked.");
            }

            return symptomList;

        }

        public List<SymptomModel> ProcessNoParam()
        {
            List<SymptomModel> symptomList = new List<SymptomModel>();

            List<SymptomModel> symptomList_VS = _restData.AQLQuery<SymptomModel>(AQLCatalog.PatientSymptom());

            if (symptomList_VS != null)
            {
                symptomList = symptomList_VS;
            }
            return symptomList;
        }

        public List<SymptomModel> PatientBySymptom(string symptom)
        {
            List<SymptomModel> symptomList = new List<SymptomModel>();

            List<SymptomModel> symptomList_VS = _restData.AQLQuery<SymptomModel>(AQLCatalog.PatientBySymptom(symptom));

            if (symptomList_VS != null)
            {
                symptomList = symptomList_VS;
            }
            return symptomList;
        }

        public List<SymptomModel> SymptomByPatient(string patientId, DateTime datum)
        {
            List<SymptomModel> symptomList = new List<SymptomModel>();

            List<SymptomModel> symptomList_VS = _restData.AQLQuery<SymptomModel>(AQLCatalog.SymptomsByPatient(patientId, datum));

            if (symptomList_VS != null)
            {
                symptomList = symptomList_VS;
            }
            return symptomList;
        }

    }
}
