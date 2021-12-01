using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.PatientMovement;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.Factories.MiBi.Contact
{
    public class ContactFactory : IContactFactory
    {
        public IRestDataAccess RestDataAccess { get; set; }
        private IPatientMovementFactory _patMoveFac;
        public ContactFactory(IRestDataAccess restDataAccess, IPatientMovementFactory patMoveFac)
        {
            RestDataAccess = restDataAccess;
            _patMoveFac = patMoveFac;
        }

        public List<Contact> Process(ContactParameter parameter)
        {
            List<MiBiView>
            List<PatientMovementModel> patientMovementModels = _patMoveFac.Process(parameter as Patient);
            Filter(ref patientMovementModels, parameter);
            foreach(PatientMovementModel patientMovement in patientMovementModels)
            {
                List<LabResult> labData = _labFac.Process();
                List<ContactPatient> contacts = _contactPatientFac.Process();
            }

            return null;
           
        }

        #region Filter
        private void Filter(ref List<PatientMovementModel> patientMovementModels, ContactParameter parameter)
        {
            if (!string.IsNullOrEmpty(parameter.CaseID))
            {
                patientMovementModels = FilterByCase(patientMovementModels, parameter.CaseID);
            }
            if (!string.IsNullOrEmpty(parameter.Ward))
            {
                patientMovementModels = FilterByWard(patientMovementModels, parameter.CaseID);
            }
            if (parameter.Start.HasValue || parameter.Start.HasValue)
            {
                patientMovementModels = FilterByTime(patientMovementModels, parameter.Start, parameter.End);
            }
        }
        private List<PatientMovementModel> FilterByWard(List<PatientMovementModel> patientMovementModels, string Ward)
        {
            return patientMovementModels.Where(m => m.StationID == Ward).ToList();
        }
        private List<PatientMovementModel> FilterByTime(List<PatientMovementModel> patientMovementModels, DateTime? Begin, DateTime? End)
        {
            if (Begin.HasValue && !End.HasValue)
            { 
                return patientMovementModels.Where(m => m.Beginn >= Begin || (m.Beginn < Begin && m.Ende >= Begin)).ToList();
            }
            else if(!Begin.HasValue && End.HasValue)
            {
                return patientMovementModels.Where(m => m.Ende <= End || (m.Ende > End && m.Beginn <= End)).ToList();
            }
            else 
            {
                return patientMovementModels.Where(m => (m.Beginn >= Begin || (m.Beginn < Begin && m.Ende >= Begin)) && (m.Ende <= End) || (m.Ende > End && m.Beginn <= End)).ToList();
            }
        }
        private List<PatientMovementModel> FilterByCase(List<PatientMovementModel> patientMovementModels, string CaseID)
        {
            return patientMovementModels.Where(m => m.FallID == CaseID).ToList();
        }
        #endregion 
    }
}
