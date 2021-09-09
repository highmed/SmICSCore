using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmICSCoreLib.AQL.Contact_Nth_Network;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.PatientInformation.Patient_Labordaten;
using SmICSCoreLib.AQL.General;

namespace SmICSWebApp.Data.Contact
{
    public class ContactService
    {
/*        public List<Contact> GetContacts(ContactModel contactInformation, Case RootPatient)
        {
            List<PatientMovementModel> RootPatientsLocations = contactInformation.PatientMovements.Where(move => move.PatientID == RootPatient.PatientID).ToList();
            List<string> wards = RootPatientsLocations.Select(location => location.StationID).ToList();

            foreach (PatientMovementModel movement in contactInformation.PatientMovements)
            {
                if(movement.PatientID != RootPatient.PatientID)
                {
                    if(wards.Contains(movement.StationID))
                    {
                        DateTime[] contactTime = OverlappingDates(movement, RootPatientsLocations);
                        if(contactTime != null)
                        {
                            List<LabDataModel> caseLabData = contactInformation.LaborData.Where(data => data.PatientID == movement.PatientID && data.FallID == movement.FallID).OrderBy(data => data.ZeitpunktProbenentnahme).ToList();
                            LabDataModel lastTest = caseLabData.Where(data => data.Antibiotika == "Oxacilin").LastOrDefault();
                            List<LabDataModel> LabaDataWhileContact = caseLabData.Where(data => data.ZeitpunktProbenentnahme <= contactTime[0] && data.ZeitpunktProbenentnahme >= contactTime[1] && data.Antibiotika == "Oxacilin" && data.Resistance == "R").FirstOrDefault();
                            new Contact
                            {
                                Begin = contactTime[0],
                                End = contactTime[1],
                                Ward = movement.StationID,
                                PatientID = movement.PatientID,
                                CaseID = movement.FallID,
                                HasFinding = lastTest.Resistance == "R" ? true : false,
                                HasFindingAtContactTime = LabaDataWhileContact == null ? false : true
                            }
                        }
                    }
                }
            }
        }*/

        private DateTime[] OverlappingDates(PatientMovementModel movement, List<PatientMovementModel> RootPatientsLocations)
        {
            dynamic stay = RootPatientsLocations.Where(move => move.StationID == movement.StationID).Select(move => new { Begin = move.Beginn, End = move.Ende });
            if(stay.End >= movement.Beginn && stay.Begin <= movement.Ende)
            {
                DateTime[] overlappingTime = new DateTime[2];
                overlappingTime[1] = stay.Begin > movement.Beginn ? stay.Begin : movement.Beginn;
                overlappingTime[1] = stay.End < movement.Ende ? stay.Ende : movement.Ende;
                return overlappingTime;
            }
            return null;
        }
    }
}
