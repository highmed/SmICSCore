using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView;
using System.Collections.Generic;

namespace SmICSWebApp.Data.MedicalFinding
{
    public class MedicalFindingService
    {
        ILabResultFactory _resultFac;

        public MedicalFindingService(ILabResultFactory resultFac)
        {
            _resultFac = resultFac;
        }

        public List<VisuLabResult> GetMedicalFinding(SmICSCoreLib.Factories.General.Patient patient)
        {
            List<VisuLabResult> labResults = new List<VisuLabResult>();
            List<LabResult> results = _resultFac.Process(patient);
            if (results != null)
            {
                foreach (LabResult result in results)
                {
                    labResults.AddRange(TransformResult(result));
                }
                return labResults;
            }
            return null;
        }

        private List<VisuLabResult> TransformResult(LabResult result)
        {
            List<VisuLabResult> results = new List<VisuLabResult>();
            foreach (Specimen specimen in result.Specimens)
            {
                foreach(Pathogen pathogen in specimen.Pathogens)
                {
                    VisuLabResult labResult = new VisuLabResult
                    {
                        LabID = specimen.LabID,
                        CaseID = result.CaseID,
                        PatientID = result.PatientID,
                        SpecimenCollectionDateTime = specimen.SpecimenCollectionDateTime,
                        SpecimenReceiptDateTime = specimen.SpecimenReceiptDate,
                        MaterialID = specimen.Kind,
                        Result = pathogen.Result,
                        Pathogen = pathogen.Name,
                        ResultDate = result.ResultDateTime,
                        Comment = result.Comment,

                    };
                    if (pathogen.MedicalField == MedicalField.VIROLOGY)
                    {
                        labResult.Quantity = pathogen.Rate;  
                    }
                    results.Add(labResult);
                }
            }
            return results;
        } 
    }
}
