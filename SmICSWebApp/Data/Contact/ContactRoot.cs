﻿using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.Nosocomial;
using SmICSCoreLib.Factories.PatientMovementNew;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using System.Collections.Generic;

namespace SmICSWebApp.Data.Contact
{
    public class ContactRoot
    {
        public ContactRoot()
        {
        }
        public List<Hospitalization> Hospitalizations { get; set; }
        public Dictionary<Hospitalization, InfectionStatus> InfectionStatus { get; set; }
        public Dictionary<Hospitalization, List<PatientStay>> PatientStays { get; set; }
        public Dictionary<Hospitalization, List<Contact>> Contacts { get; set; }

        public void AddHospitalization(Hospitalization hospitalization, InfectionStatus infectionStatus, List<PatientStay> patientStays, List<Contact> contacts)
        {
            //If contains clauses
            InfectionStatus.Add(hospitalization, infectionStatus);
            PatientStays.Add(hospitalization, patientStays);
            Contacts.Add(hospitalization, contacts);
        }
    }
}