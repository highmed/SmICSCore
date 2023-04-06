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
using Microsoft.CodeAnalysis.Elfie.Model;
using System.Diagnostics.Metrics;

namespace SmICSWebApp.Data.EpiCurve
{
    public class EpiCurveService
    {
        IHospitalizationFactory _hospitalizationFac;
        IPatientStayFactory _patStayFac;
        IInfectionStatusFactory _infectionStatusFac;

        private string ErregerID = "";
        private string ErregerBEZL = "";
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

                Dictionary<string ,InfectionStatus> infStati = infectionStatusByCase.Where(ho => ho.Key.Admission.Date == h.Admission).Select(kvp => kvp.Value).FirstOrDefault();
                if (infStati is not null)
                {
                    List<InfectionStatus> infStatus = infStati.Select(kvp => kvp.Value).ToList();

                    ErregerID = pathogen.PathogenCodesToAqlMatchString();
                    ErregerID = ErregerID.Substring(1, ErregerID.Length - 2);

                    if (infStatus is not null)
                    {
                        Dictionary<DateTime, string> AddedDates = new Dictionary<DateTime, string>();
                        foreach (InfectionStatus inf in infStatus)
                        {
                            if (inf.Known)
                            {
                                KnownHospitalization(patientStays, data, AddedDates);
                            }
                            else if (inf.Nosocomial)
                            {
                                NosocomialHospitalization(patientStays, data, inf.NosocomialDate.Value, AddedDates);
                            }
                        }
                    }
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
                    wardEpiCurve[i].MAVG7 = counts.Count > 6 ? AvgFormula(counts, 7) : 0;
                    wardEpiCurve[i].MAVG28 = counts.Count > 27 ? AvgFormula(counts, 28) : 0;
                    wardEpiCurve[i].anzahl_gesamt_av7 = completeCount.Count > 6 ? AvgFormula(completeCount, 7) : 0;
                    wardEpiCurve[i].anzahl_gesamt_av28 = completeCount.Count > 27 ? AvgFormula(completeCount, 28) : 0;
                }
            }
        }

        private int AvgFormula(List<int> values, int avgTimeFrame)
        {
            try
            {
                int startindex = values.Count - avgTimeFrame;
                List<int> subvalues = values.GetRange(startindex, avgTimeFrame);
                double sum = (double)subvalues.Sum();
                double div = Math.Ceiling(sum / avgTimeFrame);
                return (int)div;
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
                return 0;
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

        private void KnownHospitalization(List<PatientStay> stays, List<EpiCurveModel> epiCurve, Dictionary<DateTime, string> dates)
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
                if (!dates.Keys.Contains(stay.Admission.Date))
                {
                    if (admissionDay is null)
                    {
                        EpiCurveModel e = new EpiCurveModel()
                        {
                            Datum = stay.Admission,
                            StationID = stay.Ward,
                            anzahl_gesamt = 1,
                            Anzahl = 1,
                            ErregerID = this.ErregerID 
                        };
                        epiCurve.Add(e);
                    }
                    else
                    {
                        admissionDay.anzahl_gesamt++;
                        admissionDay.Anzahl++;
                    }
                    dates.Add(stay.Admission.Date, "Increment");
                }
                else if(dates.Keys.Contains(stay.Admission.Date) && dates[stay.Admission.Date] == "DECREMENT")
                {
                    admissionDay.anzahl_gesamt++;
                    admissionDay.Anzahl++;
                    dates[stay.Admission.Date] = "INCREMENT";
                }
                for(DateTime date = stay.Admission.AddDays(1.0).Date; date <= end.AddDays(-1.0).Date; date = date.AddDays(1.0).Date)
                {
                    if (!dates.Keys.Contains(date))
                    {
                        EpiCurveModel epi = epiStays.Where(e => e.Datum == date).FirstOrDefault();
                        if (epi is null)
                        {
                            EpiCurveModel e = new EpiCurveModel()
                            {
                                Datum = date,
                                StationID = stay.Ward,
                                anzahl_gesamt = 1,
                                ErregerID = this.ErregerID
                            };
                            epiCurve.Add(e);
                        }
                        else
                        {
                            epi.anzahl_gesamt++;
                        }
                        dates.Add(date, "Increment");
                    }
                    else if (dates.Keys.Contains(date.Date) && dates[date.Date] == "DECREMENT")
                    {
                        admissionDay.anzahl_gesamt++;
                        dates[date.Date] = "INCREMENT";
                    }
                }

                EpiCurveModel dischargeDay = epiStays.Where(e => e.Datum == end).FirstOrDefault();
                if (dischargeDay is null && !dates.Keys.Contains(stay.Discharge.Value.Date))
                {
                    EpiCurveModel e = new EpiCurveModel()
                    {
                        Datum = stay.Discharge.Value,
                        StationID = stay.Ward,
                        anzahl_gesamt = 0,
                        Anzahl = 0,
                        ErregerID = this.ErregerID
                    };
                    epiCurve.Add(e);
                    dates.Add(stay.Discharge.Value.Date, "DECREMENT");
                }
                else if(!dates.Keys.Contains(stay.Discharge.Value.Date)) 
                {
                    if (admissionDay.anzahl_gesamt > 0)
                    {
                        admissionDay.anzahl_gesamt--;
                    }
                    dates.Add(stay.Discharge.Value.Date, "DECREMENT");
                }

                 epiCurve.OrderBy(e => e.Datum).ThenBy(e => e.StationID);
                    
            }
        }

        private void NosocomialHospitalization(List<PatientStay> stays, List<EpiCurveModel> epiCurve, DateTime NosocomialDate, Dictionary<DateTime, string> dates)
        {
            PatientStay nosocomialStay = stays.Where(s => 
                s.Admission <= NosocomialDate && 
                ((s.Discharge.HasValue && s.Discharge.Value <= NosocomialDate) 
                || !s.Discharge.HasValue) && 
                !string.IsNullOrEmpty(s.Ward)).
                FirstOrDefault();

            EpiCurveModel nosocomialDay = epiCurve.Where(e => e.Datum == nosocomialStay.Admission).FirstOrDefault();
            if (!dates.Keys.Contains(nosocomialStay.Admission.Date))
            {
                if (nosocomialDay is null)
                {
                    EpiCurveModel e = new EpiCurveModel()
                    {
                        Datum = nosocomialStay.Admission,
                        StationID = nosocomialStay.Ward,
                        anzahl_gesamt = 1,
                        Anzahl = 1,
                        ErregerID = this.ErregerID
                    };
                    epiCurve.Add(e);
                }
                else
                {
                    nosocomialDay.anzahl_gesamt++;
                    nosocomialDay.Anzahl++;
                }
                dates.Add(nosocomialStay.Admission.Date, "Increment");
            }
            else if (dates.Keys.Contains(nosocomialStay.Admission.Date) && dates[nosocomialStay.Admission.Date] == "DECREMENT")
            {
                nosocomialDay.anzahl_gesamt++;
                nosocomialDay.Anzahl++;
                dates[nosocomialStay.Admission.Date] = "INCREMENT";
            }

            List<PatientStay> KnownStays = stays.Where(s => s.Admission >= NosocomialDate && s != nosocomialStay).ToList();
            KnownHospitalization(KnownStays, epiCurve, dates);
        }
    }
}
