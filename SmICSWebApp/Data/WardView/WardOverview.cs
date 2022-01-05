using SmICSCoreLib.Factories.MiBi.Nosocomial;
using SmICSCoreLib.Factories.MiBi.PatientView;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSWebApp.Data.WardView
{
    public class WardOverview
    {
        public InfectionStatus InfectionStatus { get; set; }
        public SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay PatientStay { get; set; }
        public List<LabResult> LabData { get; set; }

        public DateTime GetLastWardLabResultDate(DateTime Start, DateTime End)
        {
            DateTime last = DateTime.MinValue;
            foreach(LabResult labResult in LabData)
            {
                if (labResult.Sender.Ward == PatientStay.Ward)
                {
                    DateTime tmp = labResult.Specimens.OrderBy(s => s.SpecimenCollectionDateTime).Where(s => s.SpecimenCollectionDateTime >= Start && s.SpecimenCollectionDateTime <= End).Last().SpecimenCollectionDateTime;
                    if(last < tmp)
                    {
                        last = tmp;
                    }
                }

            }
            return last;
        }

        public DateTime GetFirstWardLabResultDate(DateTime Start, DateTime End)
        {
            DateTime last = DateTime.MaxValue;
            foreach (LabResult labResult in LabData)
            {
                if (labResult.Sender.Ward == PatientStay.Ward)
                {
                    DateTime tmp = labResult.Specimens.OrderBy(s => s.SpecimenCollectionDateTime).Where(s => s.SpecimenCollectionDateTime >= Start && s.SpecimenCollectionDateTime <= End).First().SpecimenCollectionDateTime;
                    if (last > tmp)
                    {
                        last = tmp;
                    }
                }

            }
            return last;
        }

        public DateTime GetLastLabResultDate(DateTime Start, DateTime End)
        {
            DateTime last = DateTime.MinValue;
            foreach (LabResult labResult in LabData)
            {
                DateTime tmp = labResult.Specimens.OrderBy(s => s.SpecimenCollectionDateTime).Where(s => s.SpecimenCollectionDateTime >= Start && s.SpecimenCollectionDateTime <= End).Last().SpecimenCollectionDateTime;
                if (last < tmp)
                {
                    last = tmp;
                }
            }
            return last;
        }
    }
}
