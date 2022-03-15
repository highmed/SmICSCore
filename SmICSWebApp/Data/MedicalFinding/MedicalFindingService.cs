using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
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

        public List<VisuLabResult> GetMedicalFinding(SmICSCoreLib.Factories.General.Patient patient, PathogenParameter pathogen)
        {
            List<VisuLabResult> labResults = new List<VisuLabResult>();
            List<LabResult> results = _resultFac.Process(patient, pathogen);
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
                        MaterialID = specimen.KindCode,
                        Material = specimen.Kind,
                        Result = pathogen.Result,
                        ResultText = pathogen.ResultText,
                        Pathogen = pathogen.Name,
                        PathogenID = pathogen.ID,
                        ResultDate = specimen.SpecimenCollectionDateTime,
                        Comment = result.Comment,
                        Quantity = pathogen.Rate,
                        Unit = string.IsNullOrEmpty(pathogen.Unit) ? null : pathogen.Unit,
                        Kind = pathogen.MedicalField == MedicalField.VIROLOGY ? "virologisch" : "mikrobiologisch"

                    };
                    results.Add(labResult);
                }
            }
            return results;
        } 
    }
}
