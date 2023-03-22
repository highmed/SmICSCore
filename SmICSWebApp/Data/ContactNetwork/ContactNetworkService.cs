using Microsoft.AspNetCore.SignalR;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.Contact;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using SmICSCoreLib.REST;
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
        private IContactFactory _contactFac;
        private PatientMovementService _movementService;
        private IPatientStayFactory _patStayFac;
        private ILabResultFactory _labFac;
        private MedicalFindingService _medicalFinding;

        private ContactModel contacts;
        private Stack<ContactNetworkParameter> ContactStack;
        private Stack<ContactNetworkParameter> nextDegree;
        private int maxDegree;
        private int currentDegree;

        public ContactNetworkService(IContactFactory contactFac, IPatientStayFactory patStayFac, ILabResultFactory labFac, MedicalFindingService medicalFinding)
        {
            _contactFac = contactFac;
            _patStayFac = patStayFac;
            _labFac = labFac;
            _medicalFinding = medicalFinding;
        }

        public async Task<ContactModel> GetContactNetwork(ContactNetworkParameter parameter)
        {

            Dictionary<Hospitalization, List<PatientStay>> contacts = await _contactFac.ProcessAsync(parameter);
            ContactModel cModel = new ContactModel();

            foreach(Hospitalization hosp in contacts.Keys)
            {
                List<PatientStay> stays = contacts[hosp];
                if (stays is not null)
                {
                    foreach (PatientStay stay in stays)
                    {
                        List<PatientStay> patientSpecificStays = await _patStayFac.ProcessAsync(stay);
                        List<VisuPatientMovement> visuPatientMovements = new List<VisuPatientMovement>();
                        foreach (PatientStay patientStay in patientSpecificStays)
                        {
                            visuPatientMovements.Add(new VisuPatientMovement(patientStay));
                        }

                        List<VisuLabResult> labResults = await _medicalFinding.GetMedicalFinding(
                            stay,
                            new SmICSCoreLib.Factories.MiBi.PatientView.Parameter.PathogenParameter()
                            {
                                PathogenCodes = new List<string> { parameter.pathogen }
                            });

                        cModel.PatientMovements.AddRange(visuPatientMovements);
                        cModel.LaborData.AddRange(labResults);
                    }
                }
            }
            return cModel;
        }

        /*
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
                            List<VisuLabResult> contactLabResults = _medicalFinding.GetMedicalFinding(contact, new SmICSCoreLib.Factories.MiBi.PatientView.Parameter.PathogenParameter()
                            {
                                PathogenCodes
                                = new List<string> { parameter.pathogen }
                            }).GetAwaiter().GetResult();
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
        */
    }
}
