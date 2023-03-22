using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.Factories.PatientMovementNew.PatientStays;
using SmICSCoreLib.Factories.PatientMovementNew;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using SmICSCoreLib.Factories.MiBi.PatientView.Parameter;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.Nosocomial;
using System.Linq;
using System.Security.Cryptography;
using Blazorise;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MudBlazor.Extensions;
using SmICSCoreLib.Util;
using static MudBlazor.CategoryTypes;

namespace SmICSWebApp.Data.EpiCurve
{
    public class EpiCurveService
    {
        IHospitalizationFactory _hospitalizationFac;
        IPatientStayFactory _patStayFac;
        IInfectionStatusFactory _infectionStatusFac;
        public EpiCurveService(IHospitalizationFactory hospitalizationFac, IPatientStayFactory patStayFac, IInfectionStatusFactory infectionStatusFac)
        {
            _hospitalizationFac = hospitalizationFac;
            _patStayFac = patStayFac;
            _infectionStatusFac = infectionStatusFac;
        }

        public async Task<List<EpiCurveModel>> GetData(DateTime start, DateTime end, PathogenParameter pathogen)
        {
            List<EpiCurveModel> data = new List<EpiCurveModel>();

            List<HospStay> hosps = await _hospitalizationFac.ProcessAsync(start, end);

            foreach(HospStay h in hosps)
            {
                SortedList<Hospitalization, Dictionary<string, InfectionStatus>> infectionStatusByCase = await _infectionStatusFac.ProcessAsync(h, pathogen);
                List<PatientStay> patientStays = await _patStayFac.ProcessAsync(h);

                InfectionStatus infStatus = infectionStatusByCase.Where(ho => ho.Key.Admission.Date == h.Admission).First().Value["resitenz"];
                //resitenz ändern
                if (infStatus.Known)
                {
                    KnownHospitalization(patientStays, data);
                }
                else if (infStatus.Nosocomial)
                {
                    NosocomialHospitalization(patientStays, data, infStatus.NosocomialDate.Value);
                }
            }
            CalculateKlinik(data, start, end);
            CalculateMAVG(data);
            return data;
        }

        private void CalculateMAVG(List<EpiCurveModel> epiCurve)
        {
            List<string> wards = epiCurve.GroupBy(e => e.StationID).Select(g => g.Key).ToList();
            foreach (string ward in wards)
            {
                List<EpiCurveModel> wardEpiCurve = epiCurve.Where(e => e.StationID == ward).ToList();
                List<int> counts = new List<int>();
                List<int> completeCount = new List<int>();
                for (int i = 0; i < wardEpiCurve.Count; i++)
                {
                    counts.Add(wardEpiCurve[i].Anzahl);
                    completeCount.Add(wardEpiCurve[i].anzahl_gesamt);
                    wardEpiCurve[i].MAVG7 = counts.Count > 6 ? (int)Math.Ceiling((double)counts.GetRange(counts.Count - 7, counts.Count - 1).Sum() / 7) : 0;
                    wardEpiCurve[i].MAVG28 = counts.Count > 27 ? (int)Math.Ceiling((double)counts.GetRange(counts.Count - 28, counts.Count - 1).Sum() / 28) : 0;
                    wardEpiCurve[i].anzahl_gesamt_av7 = completeCount.Count > 6 ? (int)Math.Ceiling((double)completeCount.GetRange(completeCount.Count - 7, completeCount.Count - 1).Sum() / 7) : 0;
                    wardEpiCurve[i].anzahl_gesamt_av28 = completeCount.Count > 27 ? (int)Math.Ceiling((double)completeCount.GetRange(completeCount.Count - 28, completeCount.Count - 1).Sum() / 28) : 0;
                }
            }
        }

        private void CalculateKlinik(List<EpiCurveModel> epiCurve, DateTime start, DateTime end)
        {
            for (DateTime date = start.Date; date.Date <= end.Date; date = date.AddDays(1.0).Date)
            {
                List<EpiCurveModel> epi = epiCurve.Where(d => d.Datum == date).ToList();
                EpiCurveModel klinik = new EpiCurveModel()
                {
                    StationID = "klinik",
                    Datum = date,
                    Anzahl = epi.Sum(e => e.Anzahl),
                    anzahl_gesamt = epi.Sum(e => e.anzahl_gesamt)
                };
                epiCurve.Add(klinik);
            }
        }

        private void KnownHospitalization(List<PatientStay> stays, List<EpiCurveModel> epiCurve)
        {
            foreach(PatientStay stay in stays)
            {
                DateTime now = DateTime.Now;
                        
                // GESAMT: funktioniert das so, werden die referenzen dennoch weiter gegeben auch bei listen oder muss ich irgendwie das originalobjekt aus der übergebenen liste holen
                List<EpiCurveModel> epiStays = epiCurve.Where(e => 
                e.StationID == stay.Ward && 
                e.Datum >= stay.Admission && 
                ((stay.Discharge.HasValue && e.Datum <= stay.Discharge.Value) || 
                (stay.Discharge.HasValue && e.Datum <= now))).ToList();
                epiCurve = epiCurve.OrderBy(e => e.Datum).ThenBy(e => e.StationID).ToList();
                epiStays = epiStays.OrderBy(e => e.Datum).ToList();

                DateTime end = stay.Discharge.HasValue ? stay.Discharge.Value : now;
                EpiCurveModel admissionDay = epiStays.Where(e => e.Datum == stay.Admission).FirstOrDefault();
                if (admissionDay is null) 
                {
                    EpiCurveModel e = new EpiCurveModel()
                    {
                        Datum = stay.Admission,
                        StationID = stay.Ward,
                        anzahl_gesamt = 1,
                        Anzahl = 1
                    };
                    epiCurve.Add(e);
                }
                else
                {
                    admissionDay.anzahl_gesamt++;
                    admissionDay.Anzahl++;
                }
                
                for(DateTime date = stay.Admission.AddDays(1.0).Date; date <= end.AddDays(-1.0).Date; date = date.AddDays(1.0).Date)
                {
                    EpiCurveModel epi = epiStays.Where(e => e.Datum == date).FirstOrDefault();
                    if(epi is null)
                    {
                        EpiCurveModel e = new EpiCurveModel()
                        {
                            Datum = date,
                            StationID = stay.Ward,
                            anzahl_gesamt = 1
                        };
                        epiCurve.Add(e);
                    }
                    else 
                    {
                        epi.anzahl_gesamt++;
                    }
                }

                EpiCurveModel dischargeDay = epiStays.Where(e => e.Datum == end).FirstOrDefault();
                if (admissionDay is null)
                {
                    EpiCurveModel e = new EpiCurveModel()
                    {
                        Datum = stay.Admission,
                        StationID = stay.Ward,
                        anzahl_gesamt = 0,
                        Anzahl = 0
                    };
                    epiCurve.Add(e);
                }
                else
                {
                    if (admissionDay.anzahl_gesamt > 0)
                    {
                        admissionDay.anzahl_gesamt--;

                    }
                }

                epiCurve.OrderBy(e => e.Datum).ThenBy(e => e.StationID);

            }
        }

        private void NosocomialHospitalization(List<PatientStay> stays, List<EpiCurveModel> epiCurve, DateTime NosocomialDate)
        {
            PatientStay nosocomialStay = stays.Where(s => 
                s.Admission <= NosocomialDate && 
                ((s.Discharge.HasValue && s.Discharge.Value <= NosocomialDate) 
                || !s.Discharge.HasValue) && 
                !string.IsNullOrEmpty(s.Ward)).
                FirstOrDefault();

            List<PatientStay> KnownStays = stays.Where(s => s.Admission >= NosocomialDate && s != nosocomialStay).ToList();
            KnownHospitalization(KnownStays, epiCurve);
        }
    }
}
