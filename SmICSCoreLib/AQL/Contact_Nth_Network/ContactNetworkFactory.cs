using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmICSCoreLib.AQL.Contact_Nth_Network.ReceiveModels;
using SmICSCoreLib.REST;
using SmICSCoreLib.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Contact_Nth_Network
{
    public class ContactNetworkFactory : IContactNetworkFactory
    {
        private static Stack<ContactParameter> patientStack;
        private static List<ContactModel> contactList;
        private static int currentDegree;

        private IRestDataAccess _restData;

        public ContactNetworkFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }

        public List<ContactModel> Process(ContactParameter parameter)
        {
            patientStack = new Stack<ContactParameter>();
            contactList = new List<ContactModel>();
            currentDegree = 1;

            patientStack.Push(parameter);

            DegreeIterator();
            return contactList;
        }

        private void DegreeIterator()
        {
            int maxDegree = Convert.ToInt32(patientStack.Peek().Degree);
            contactList = new List<ContactModel>();

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
            List<PatientWardModel> patientWardList = _restData.AQLQuery<PatientWardModel>(AQLCatalog.ContactPatientWards(parameter));

            if (patientWardList is null)
            {
                return;
            }

            FindContactPatients(patientWardList, parameter);
        }

        private void FindContactPatients(List<PatientWardModel> PatientWardList, ContactParameter parameter)
        {
            
            foreach (PatientWardModel patientWard in PatientWardList)
            {
                ContactPatientsParameter secondQueryParameter = SecondParameterConstructor(patientWard, parameter);
                List<ContactPatientModel> contactPatientList = _restData.AQLQuery<ContactPatientModel>(AQLCatalog.ContactPatients(secondQueryParameter));
                if (contactPatientList == null)
                {
                    continue;
                }
                
                ContactModelConstructor(contactPatientList, patientWard, parameter);
            }
        }

        private void ContactModelConstructor(List<ContactPatientModel> ContactPatientList, PatientWardModel patientWard, ContactParameter parameter)
        {
            foreach (ContactPatientModel contactPatient in ContactPatientList)
            {
                if (isValidTimeSlot(contactPatient, parameter) == false)
                {
                    continue;
                }

                ContactModel contact = new ContactModel(contactPatient, patientWard, parameter, currentDegree);
                if (!contactList.Contains(contact))
                {
                    contactList.Add(contact);
                    updatePatientStack(contactPatient, parameter);
                }

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
