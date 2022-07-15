using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using SmICSWebApp.Data.MedicalFinding;
using SmICSWebApp.Data.PatientMovement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmICSWebApp.Data.ContactNetwork
{
    public class ContactNetworkService
    {
        private MedicalFindingService _medicalFinding;
        private PatientMovementService _movementService;
        private IPatientStayFactory _patStayFac;

        private ContactModel contacts;
        private Stack<ContactNetworkParameter> ContactStack;
        private Stack<ContactNetworkParameter> nextDegree;
        private int maxDegree;
        private int currentDegree;

        public ContactNetworkService(MedicalFindingService medicalFindingService, PatientMovementService patientMovementService, IPatientStayFactory patientStayFactory)
        {
            _medicalFinding = medicalFindingService;
            _movementService = patientMovementService;
            _patStayFac = patientStayFactory;
        }

        public ContactModel GetContactNetwork(ContactNetworkParameter parameter)
        {
            try
            {
                ContactStack = new Stack<ContactNetworkParameter>();
                ContactStack.Push(parameter);

                contacts = new ContactModel() { PatientMovements = new List<VisuPatientMovement>(), LaborData = new List<VisuLabResult>() };

                int maxDegree = Convert.ToInt32(ContactStack.Peek().Degree);
                int currentDegree = 1;

                while (currentDegree <= maxDegree)
                {
                    nextDegree = new Stack<ContactNetworkParameter>();
                    while (ContactStack.Count > 0)
                    {
                        FindWardsQuery();
                    }
                    currentDegree += 1;
                    ContactStack = nextDegree;
                }
                return contacts;
            }
            catch
            {
                throw;

            }
        }

        private void FindWardsQuery()
        {
            ContactNetworkParameter parameter = ContactStack.Pop();
            List<VisuPatientMovement> patientWardList = _movementService.GetPatientMovements(parameter).GetAwaiter().GetResult();
            patientWardList = patientWardList.Where(w => w.MovementTypeID != (int)MovementType.ADMISSION && w.MovementTypeID != (int)MovementType.DISCHARGE).ToList();

            if (patientWardList is null)
            {
                return;
            }

            FindContactPatients(patientWardList, parameter);
        }


        private void FindContactPatients(List<VisuPatientMovement> PatientWardList, ContactNetworkParameter parameter)
        {

            foreach (VisuPatientMovement patientWard in PatientWardList)
            {
                List<PatientStay> contactStays = _patStayFac.ProcessAsync(new WardParameter() { Ward = patientWard.Ward, Start = patientWard.Admission, End = patientWard.Discharge }).GetAwaiter().GetResult();
                if (contactStays is not null)
                {
                    foreach (PatientStay contact in contactStays)
                    {
                        if (contacts.PatientMovements.Where(c => c.PatientID == contact.PatientID).Count() == 0)
                        {
                            List<VisuLabResult> contactLabResults = _medicalFinding.GetMedicalFinding(contact, new SmICSCoreLib.Factories.MiBi.PatientView.Parameter.PathogenParameter() { PathogenCodes
                                = new List<string> { parameter.pathogen }    }).GetAwaiter().GetResult();
                            List<VisuPatientMovement> contactMovements = _movementService.GetPatientMovements(contact).GetAwaiter().GetResult();

                            contacts.LaborData.AddRange(contactLabResults);
                            contacts.PatientMovements.AddRange(contactMovements);

                            if (currentDegree < maxDegree)
                            {
                                nextDegree.Push(new ContactNetworkParameter
                                {
                                    PatientID = contact.PatientID,
                                    Degree = maxDegree,
                                    pathogen = parameter.pathogen,
                                    starttime = contactMovements.First().Admission,
                                    endtime = contact.Discharge.HasValue && contact.Discharge.Value > patientWard.Discharge ? contact.Discharge.Value : patientWard.Discharge
                                });
                            }
                        }
                    }
                }
            }
        }
    }
}
