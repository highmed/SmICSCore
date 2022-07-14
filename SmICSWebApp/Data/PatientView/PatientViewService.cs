using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.PatientMovementNew;
using System.Collections.Generic;
using System.Linq;

namespace SmICSWebApp.Data.PatientView
{
    public class PatientViewService
    {
        private readonly ILabResultFactory _labDataFac;
        private readonly IHospitalizationFactory _hospFac;

        public PatientViewService(ILabResultFactory labDataFac, IHospitalizationFactory hospFac)
        {
            _labDataFac = labDataFac;
            _hospFac = hospFac;
        }

        public SortedDictionary<Hospitalization, List<LabResult>> GetData(SmICSCoreLib.Factories.General.Patient patient)
        {
            try
            {
                SortedDictionary<Hospitalization, List<LabResult>> patientHistory = new SortedDictionary<Hospitalization, List<LabResult>>();
                List<Hospitalization> hospitalizations = _hospFac.Process(patient);
                if (hospitalizations is not null)
                {
                    hospitalizations = hospitalizations.OrderByDescending(h => h.Admission.Date).ToList();
                    foreach (Hospitalization hosp in hospitalizations)
                    {
                        List<LabResult> labs = _labDataFac.Process(hosp, MedicalField.MICROBIOLOGY);
                        if (labs is not null)
                        {
                            labs = labs.OrderByDescending(l => l.Specimens.Min(s => s.SpecimenCollectionDateTime)).ToList();
                            patientHistory.Add(hosp, labs);
                        }
                    }
                    return patientHistory;
                }
                return null;
            }
            catch 
            {
                throw;
            }
        }
    }
}
