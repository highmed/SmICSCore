using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmICSCoreLib.Factories.ContactNetwork.ReceiveModels;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.Lab.ViroLabData;
using SmICSCoreLib.Factories.PatientMovement;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.ContactNetwork
{
    public class ContactNetworkFactory : IContactNetworkFactory
    {
        private static Stack<ContactParameter> patientStack;
        private static ContactModel contacts;
        private static int currentDegree;

        private readonly ILogger<ContactNetworkFactory> _logger;
        private readonly IPatientMovementFactory _patMoveFac;
        private readonly IViroLabDataFactory _patLabFac;

        public IRestDataAccess RestDataAccess { get; }

        public ContactNetworkFactory(IRestDataAccess restData, IPatientMovementFactory patMoveFac, IViroLabDataFactory patLabFac, ILogger<ContactNetworkFactory> logger)
        {
            _logger = logger;
            RestDataAccess = restData;
            _patLabFac = patLabFac;
            _patMoveFac = patMoveFac;
        }

        public ContactModel Process(ContactParameter parameter)
        {
            patientStack = new Stack<ContactParameter>();
            contacts = new ContactModel() { PatientMovements = new List<PatientMovementModel>(), LaborData = new List<LabDataModel>() };
            currentDegree = 1;

            patientStack.Push(parameter);
            
            DegreeIterator();
            _logger.LogDebug(contacts.PatientMovements.ToString());
            _logger.LogDebug(contacts.LaborData.ToString());
            return contacts;
        }

        private void DegreeIterator()
        {
            int maxDegree = Convert.ToInt32(patientStack.Peek().Degree);

            while (currentDegree <= maxDegree)
            {
                Stack<string> newPatientStack = new Stack<string>();

                while (patientStack.Count > 0)
                {
                    FindWardsQuery();
                }
                currentDegree += 1;
            }
        }

        private void FindWardsQuery()
        {
            ContactParameter parameter = patientStack.Pop();
            List<PatientWardModel> patientWardList = RestDataAccess.AQLQuery<PatientWardModel>(AQLCatalog.ContactPatientWards(parameter));

            if (patientWardList is null || patientWardList.Count == 0)
            {
                _logger.LogDebug("ContactNetworkFactory.FindWardsQuery(): Found No Wards - ResultSet: NULL");
                return;
            }

            FindContactPatients(patientWardList, parameter);
        }

        private void FindContactPatients(List<PatientWardModel> PatientWardList, ContactParameter parameter)
        {
            
            foreach (PatientWardModel patientWard in PatientWardList)
            {
                ContactPatientsParameter secondQueryParameter = SecondParameterConstructor(patientWard, parameter);
                List<ContactPatientModel> contactPatientList  = null;
                if (patientWard.StationID == null)
                {
                    _logger.LogInformation("ContactNetworkFactory.FindContactPatients(): No WardID From ContactNetworkFactory.FindWardsQuery(). Set DepartementID to WardID.");
                    secondQueryParameter.WardID = patientWard.Fachabteilung;
                    contactPatientList = RestDataAccess.AQLQuery<ContactPatientModel>(AQLCatalog.ContactPatients_WithoutWardInformation(secondQueryParameter));
                }
                else
                {
                    contactPatientList = RestDataAccess.AQLQuery<ContactPatientModel>(AQLCatalog.ContactPatients(secondQueryParameter));
                }
                if (contactPatientList == null)
                {
                    _logger.LogDebug("ContactNetworkFactory.FindContactPatients(): Found No Contact Patients For Ward {wardID} - ResultSet: NULL", secondQueryParameter.WardID);
                    continue;
                }
                
                ContactModelConstructor(contactPatientList);
            }
        }

        private void ContactModelConstructor(List<ContactPatientModel> ContactPatientList)
        {
            foreach (ContactPatientModel contact in ContactPatientList)
            {
                PatientListParameter patList = new PatientListParameter() { patientList = new List<string> { contact.PatientID } };
                contacts.PatientMovements.AddRange(_patMoveFac.Process(patList));
                contacts.LaborData.AddRange(_patLabFac.Process(patList));
            }
        }
        
        private ContactPatientsParameter SecondParameterConstructor(PatientWardModel patientWard, ContactParameter baseParameter)
        {
            ContactPatientsParameter contactPatients = new ContactPatientsParameter(baseParameter, patientWard);
            return contactPatients;
        }

        private bool isValidTimeSlot(ContactPatientModel contactPatient, ContactParameter parameter)
        {
            return (contactPatient.Ende > parameter.Starttime || contactPatient.Beginn < parameter.Endtime);
        }

        private void updatePatientStack(ContactPatientModel contactPatient, ContactParameter parameter)
        {
            if (currentDegree < parameter.Degree)
            {
                ContactParameter newContractParameter = new ContactParameter();
                newContractParameter.Starttime = parameter.Starttime;
                newContractParameter.Endtime = parameter.Endtime;
                newContractParameter.Degree = parameter.Degree - 1;
                newContractParameter.PatientID = contactPatient.PatientID;

                patientStack.Push(newContractParameter);
            }
        }
    }
}
