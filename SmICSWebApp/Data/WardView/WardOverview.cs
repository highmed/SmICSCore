using SmICSCoreLib.Factories.MiBi.Nosocomial;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.PatientInformation.PatientData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmICSWebApp.Data.WardView
{
    public class WardOverview
    {
        public Dictionary<string, InfectionStatus> InfectionStatus { get; set; }
        public SmICSCoreLib.Factories.PatientMovementNew.PatientStays.PatientStay PatientStay { get; set; }
        public List<LabResult> LabData { get; set; }
        public PatientData PatientData { get; set; }

        public DateTime? GetLastWardLabResultDate(DateTime Start, DateTime End)
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
            return last == DateTime.MinValue ? null : last;
        }

        public DateTime? GetFirstWardLabResultDate(DateTime Start, DateTime End)
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
            return last == DateTime.MaxValue ? null : last;
        }

        public DateTime? GetFirstPositveLabResultDate()
        {
            DateTime last = DateTime.MaxValue;
            foreach (LabResult labResult in LabData)
            {
                if (labResult.Sender.Ward == PatientStay.Ward)
                {
                    DateTime? tmp = labResult.Specimens.
                        OrderBy(s => s.SpecimenCollectionDateTime).
                        Where(s => s.Pathogens.Any(p => p.Result)).
                        Select(s => s.SpecimenCollectionDateTime).
                        FirstOrDefault();
                    if (tmp.HasValue && last > tmp.Value)
                    {
                        last = tmp.Value;
                    }
                }

            }
            return last == DateTime.MaxValue ? null : last;
        }

        public DateTime? GetLastLabResultDate()
        {
            DateTime last = DateTime.MinValue;
            foreach (LabResult labResult in LabData)
            {
                DateTime tmp = labResult.Specimens.OrderBy(s => s.SpecimenCollectionDateTime).Last().SpecimenCollectionDateTime;
                if (last < tmp)
                {
                    last = tmp;
                }
            }
            return last == DateTime.MinValue ? null : last;
        }
    }
}
