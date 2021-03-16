using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmICSCoreLib.AQL.Contact_Nth_Network;
using SmICSCoreLib.REST;
using SmICSCoreLib.Util;

namespace SmICSCoreLib.AQL.unused
{
    public class StoredProcedures
    {
        /*#region globale Variables
        private RestClient client = null;
        private StationNameTransformation stationTransformation = null;
        private PathogenNameTransformation pathogenTransformation = null;
        private MaterialNameTransformation materialTransformation = null;
        private ParameterTransformation parameterTransformation = null;
        private PayloadControl payloadControl = null;
        private PayloadValidator PayloadValidator = null;
        #endregion
        public StoredProcedures(RestClient restClient)
        {
            this.client = restClient;
            //Hopefully aren't needed more for MiBi - Version
            this.stationTransformation = new StationNameTransformation(restClient);
            this.pathogenTransformation = new PathogenNameTransformation(restClient);
            this.materialTransformation = new MaterialNameTransformation(restClient);
            this.parameterTransformation = new ParameterTransformation();
            PayloadValidator = new PayloadValidator();
            //

            this.payloadControl = new PayloadControl();
        }

        public StoredProcedures()
        {
        }
        #region Patient Information
      
        /// <summary>
        /// Calls the query for the complete lab data for the given patients and transforms the result to the following structure. Returns JSON:
        /// [
        ///     {
        ///         LabordatenID": int, 
        ///         "PatientID": int,
        ///         "FallID": int, 
        ///         "ProbeID": int, 
        ///         "ZeitpunktProbenentnahme": DateTime, 
        ///         "ZeitpunktProbeneingang": DateTime, 
        ///         "MaterialID": int, 
        ///         "Material_k": string, 
        ///         "Material_l": string, 
        ///         "Befund": string,
        ///         "Befundkommentar": string
        ///     }
        /// ]
        /// </summary>
        /// <param name="patientList"></param>
        /// <returns></returns>
        public JArray Patient_Labordaten_Ps(string patientList)
        {
            try
            {
                this.payloadControl.checkStringParam(patientList, PayloadControl.Pattern.LIST);
                patientList = this.parameterTransformation.ConvertToMatchesString(patientList);
                AQLQuery q = AQLCatalog.Patient_Labordaten_Ps(patientList);
                JArray result = this.client.AQLQuery(q.Query);

                //if the query returns no result set a null object will returend to trigger a status code 204
                if (result is null)
                {
                    return new JArray();
                }
                else
                {
                    //Converting some of the JSON results to different data type
                    foreach (JObject obj in result)
                    {
                        this.parameterTransformation.ConvertToInt64(obj, new List<string> { "FallID", "LabordatenID", "ProbeID", "PatientID" });
                        this.parameterTransformation.ConvertToBoolean(obj, "Befund");
                        this.pathogenTransformation.ConvertToInt(obj, "Keim_k", "KeimID");
                        this.materialTransformation.ConvertToInt(obj, "Material_k", "MaterialID");
                    }
                    System.Diagnostics.Debug.Print(result.ToString());
                    return result;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.ToString());
                throw this.parameterTransformation.CastException(e);
            }
        }

        /// <summary>
        /// Calls the query for the pathogen flags which were manually set by the medical staff  and transforms the result to the following structure. Returns JSON:
        /// [
        ///     {
        ///         "PatientID": int,
        ///         "TimeStamp": DateTime,
        ///         "Flag": bool
        ///     }
        /// ]
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="pathogenID"></param>
        /// <param name="patientList"></param>
        /// <returns>JArray</returns>
        public JArray Patient_PathogenFlag_TTEPs(string starttime, string endtime, int pathogenID, string patientList)
        {
            try 
            {
                //controls if every parameter is valid.
                //this.payloadControl.checkDateStringParam(starttime, PayloadControl.Pattern.DATETIME);
                //this.payloadControl.checkDateStringParam(endtime, PayloadControl.Pattern.DATETIME);
                this.payloadControl.checkStringParam(patientList, PayloadControl.Pattern.LIST);

                //Converting the ID of the pathogen to the alias of the pathogen, because the AQL just works with the pathogen alias
                //the ID is a constructed ID within SmICS
                string pathogen = this.pathogenTransformation.getPathogenName(pathogenID);
                patientList = this.parameterTransformation.ConvertToMatchesString(patientList);

                AQLQuery q = AQLCatalog.Patient_PathogenFlag_TTEPs(starttime, endtime, pathogen, patientList);
                JArray result = this.client.AQLQuery(q.Query);

                //if the query returns no result set a null object will returend to trigger a status code 204
                if (result is null)
                {
                    return new JArray();
                }
                else
                {
                    //Converting some of the JSON results to different data type
                    foreach (JObject obj in result)
                    {
                        this.parameterTransformation.ConvertToInt64(obj, new List<string>() { "PatientID" });
                        this.parameterTransformation.ConvertToBoolean(obj, "Flag");
                        obj.Add("KeimID", 1);
                    }
                    System.Diagnostics.Debug.Print(result.ToString());
                    return result;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.ToString());
                throw this.parameterTransformation.CastException(e);
            }
        }

        /// <summary>
        /// Calls the query for the Diagnostic Result of the given patients, pathogen and timespan and transforms the result to the following structure. Returns JSON:
        /// [
        ///     {
        ///         "PatientID": int,
        ///         "ICD_Code": string,
        ///         "Diagnose": string,
        ///         "Freitextbeschreibung": string,
        ///         "TimeStamp": DateTime,
        ///         "Zeitpunkt_der_Genesung"; DateTime   
        ///     }
        /// ]
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <param name="patientList"></param>
        /// <returns></returns>
        public JArray Patient_DiagnosticResults_TTPs(string starttime, string endtime, string patientList)
        {
            try
            {
                //controls if every parameter is valid. 
                this.payloadControl.checkStringParam(patientList, PayloadControl.Pattern.LIST);
                //this.payloadControl.checkDateStringParam(starttime, PayloadControl.Pattern.DATETIME);
                //this.payloadControl.checkDateStringParam(endtime, PayloadControl.Pattern.DATETIME);
                patientList = this.parameterTransformation.ConvertToMatchesString(patientList);

                AQLQuery q = AQLCatalog.Patient_DiagnosticResults_TTPs(starttime, endtime, patientList);
                System.Diagnostics.Debug.Print(q.Query);
                JArray result = this.client.AQLQuery(q.Query);

                //if the query returns no result set a null object will returend to trigger a status code 204
                if (result is null)
                {
                    return new JArray();
                }
                else
                {
                    foreach(JObject obj in result)
                    {
                        this.parameterTransformation.ConvertToInt64(obj, new List<string> { "PatientID" });
                        obj.Add("KeimID", 1);
                    }
                    System.Diagnostics.Debug.Print(result.ToString());
                    return result;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.ToString());
                throw this.parameterTransformation.CastException(e);
            }
        }

        /// <summary>
        /// Calls the query for the complete movements of the given patients and timespan and transforms the result to the following structure. Returns JSON: 
        /// [
        ///     {
        ///         "PatientID": int,
        ///         "FallID": int,
        ///         "StationID": int,
        ///         "Beginn": DateTime,
        ///         "Ende": DateTime,
        ///         "Bewegungsart": string,
        ///         "BewegungstypID": int,
        ///         "Bewegungstyp": string,
        ///     }
        /// ]
        /// </summary>
        /// <param name="patientList"></param>
        /// <returns>JArray</returns>
        public JArray Patient_Bewegung_Ps(string patientList)
        {
            try
            {
                //catches wrong parameter input
                this.payloadControl.checkStringParam(patientList, PayloadControl.Pattern.LIST);

                //Converts the incoming patientList to a patinetList which can be interpreted by openEHR AQL
                patientList = this.parameterTransformation.ConvertToMatchesString(patientList);
                System.Diagnostics.Debug.Print("PatientList: " + patientList);
                //Sends query to the openEHR server and return the resultSet
                AQLQuery q = AQLCatalog.Patient_Bewegung_Ps(patientList);
                System.Diagnostics.Debug.Print(q.Query);
                JArray result = this.client.AQLQuery(q.Query);

                //if the query returns no result set a null object will returend to trigger a status code 204
                if (result is null)
                {
                    return new JArray();
                }

                //List for concated patientID and caseID 
                List<string> pat_fall = new List<string>();

                //Since this time two queries are need and the result sets have to be merged an empty JArray as return value is needed. All merged results will be added to this JArray
                JArray retArr = new JArray();
                JObject start_end_Obj = null;

                foreach (JObject obj in result)
                {
                    //concats identifiers
                    string patfallID = obj.Property("PatientID").Value.ToString() + obj.Property("FallID").Value.ToString();

                    //every case of a patient just needs to be checked ones if there are an admission- and/or discharge timestamp
                    if (!pat_fall.Contains(patfallID))
                    {
                        pat_fall.Add(patfallID);
                        AQLQuery q2 = AQLCatalog.Patient_Bewegung_Ps_II(obj.Property("PatientID").Value.ToString(), obj.Property("FallID").Value.ToString());
                        JArray result2 = this.client.AQLQuery(q2.Query);
                        if (!(result2 is null))
                        {
                            //result.First because there can be just one admission/discharge timestamp for each case
                            start_end_Obj = (JObject)result2.First;
                        }
                    }

                    //Converting some of the JSON results to different data type
                    this.parameterTransformation.ConvertToInt64(obj, new List<string> { "PatientID", "FallID" });
                    this.stationTransformation.ConvertToStationID(obj);

                    //Adding Bewegungstyp and Bewegungstyp by comparing start- end end dates of each object and contructs special objects for admission and discharge
                    this.addAdmissionObject(obj, start_end_Obj, retArr);
                    this.addMovementTypeByDateComparison(obj);
                    this.addDischargeObject(obj, start_end_Obj, retArr);

                    retArr.Add(obj);                    
                }
                System.Diagnostics.Debug.Print(retArr.ToString());
                return retArr;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print(e.ToString());
                throw this.parameterTransformation.CastException(e);
            }
        }

        #region Helpers: Patient_Bewegung_Ps
        private void addMovementTypeByDateComparison(JObject patientMovement)
        {
            if (patientMovement.Property("Beginn").Value.ToString() == patientMovement.Property("Ende").Value.ToString() || patientMovement.Property("Ende").Value.ToString() == "")
            {
                if (patientMovement.Property("Ende").Value.ToString() == "")
                {
                    patientMovement.Property("Ende").Value = patientMovement.Property("Beginn").Value;
                }
                this.addMovementType(patientMovement, 4, "Behandlung");
            }
            else
            {
                this.addMovementType(patientMovement, 3, "Wechsel");
            }
        }
        private void addAdmissionObject(JObject patientMovement, JObject admissionDischarge, JArray returnArray)
        {
            if (!(admissionDischarge is null) && patientMovement.Property("Beginn").Value.ToString() == admissionDischarge.Property("Beginn").Value.ToString())
            {
                JObject start = new JObject(patientMovement);
                start.Property("Ende").Value = start.Property("Beginn").Value;
                this.addMovementType(start, 1, "Aufnahme");
                returnArray.Add(start);
            }

        }
        private void addDischargeObject(JObject patientMovement, JObject admissionDischarge, JArray returnArray)
        {
            if (!(admissionDischarge is null) && patientMovement.Property("Ende").Value.ToString() == admissionDischarge.Property("Ende").Value.ToString())
            {
                JObject end = new JObject(patientMovement);
                end.Property("Beginn").Value = end.Property("Ende").Value;
                end.Property("BewegungstypID").Value = 2;
                end.Property("Bewegungstyp").Value = "Entlassung";
                returnArray.Add(end);
            }
        }

        private void addMovementType(JObject obj, int typeID, string typeName)
        {
            obj.Add("BewegungstypID", typeID);
            obj.Add("Bewegungstyp", typeName);
        }
        #endregion
       
        #endregion

        #region Lab
        public JArray Labor_Epikurve(string starttime, string endtime)
        {
            JArray retArr = new JArray();

            AQLQuery q = AQLCatalog.Labor_Epikurve(starttime, endtime, "true");
            JArray result = this.client.AQLQuery(q.Query);

            AQLQuery q2 = AQLCatalog.Labor_Epikurve(starttime, endtime, "false");
            JArray result2 = this.client.AQLQuery(q2.Query);

            if (result is null) { return new JArray(); }

            SortedDictionary<DateTime, int> active = new SortedDictionary<DateTime, int>();
            SortedDictionary<DateTime, int> inactive = new SortedDictionary<DateTime, int>();
            SortedDictionary<DateTime, int> sum = new SortedDictionary<DateTime, int>();

            DateTime start = Convert.ToDateTime(starttime);
            DateTime end = Convert.ToDateTime(endtime);

            //Prefills the dictionaries
            for (DateTime date = start.Date; date.Date <= end.Date; date = date.AddDays(1))
            {
                active.Add(date, 0);
                inactive.Add(date, 0);
                sum.Add(date, 0);
            }

            //
            foreach (JObject obj in result)
            {
                DateTime datetime = this.parameterTransformation.ConvertToDateTime(obj, "Zeitpunkt");
                int dayCount = Convert.ToInt32(obj.Property("Flag").Value);
                active[datetime.Date] += dayCount;
                sum[datetime.Date] += dayCount;
            }

            foreach (JObject obj in result2)
            {
                DateTime datetime = this.parameterTransformation.ConvertToDateTime(obj, "Zeitpunkt");
                sum[datetime.Date] -= Convert.ToInt32(obj.Property("Flag").Value);
            }

            List<int> last7days = new List<int>();
            List<int> last7days_overall = new List<int>();
            List<int> last28days = new List<int>();
            List<int> last28days_overall = new List<int>();

            for (DateTime date = start.Date; date.Date <= end.Date; date = date.AddDays(1))
            {
                if (date.Date < end.Date)
                {
                    sum[date.AddDays(1.0)] += sum[date];
                }

                JObject newObj = new JObject();

                newObj.Add("Datum", date);
                newObj.Add("ErregerBEZK", "COV");
                newObj.Add("Anzahl", active[date]);
                newObj.Add("Anzahl_cs", 0);
                newObj.Add("MAVG7", MovingAverage(last7days, active[date], 7));
                newObj.Add("MAVG28", MovingAverage(last28days, active[date], 28));
                this.pathogenTransformation.ConvertToInt(newObj, "ErregerBEZK", "ErregerID");
                newObj.Add("StationID", 0);
                newObj.Add("anzahl_gesamt", 0);
                newObj.Add("anzahl_gesamt_av7", MovingAverage(last7days_overall, sum[date], 7));
                newObj.Add("anzahl_gesamt_av28", MovingAverage(last28days_overall, sum[date], 28));

                JObject clinicObj = new JObject(newObj);

                clinicObj.Property("StationID").Value = "klinik";
                clinicObj.Property("anzahl_gesamt").Value = sum[date];

                retArr.Add(newObj);
                retArr.Add(clinicObj);
            }

            return retArr;
        }

        private int MovingAverage(List<int> count, int daycount, int mavgTime)
        {
            int mavg = 0;
            count.Add(daycount);
            if (count.Count == mavgTime)
            {
                mavg = (int)Math.Ceiling((double)count.Sum() / mavgTime);
                count.RemoveAt(0);
            }
            return mavg;
        }

        #endregion

        #region RKI_Algorithm

        public JArray RKI_Dataset(string starttime, string endtime)
        {
            JArray retArr = new JArray();

            AQLQuery q = AQLCatalog.Labor_Epikurve(starttime, endtime, "true");
            JArray result = this.client.AQLQuery(q.Query);

            if (result is null) { return new JArray(); }

            DateTime start = Convert.ToDateTime(starttime);
            DateTime end = Convert.ToDateTime(starttime);

            SortedDictionary<int, int> pathoCountperWeeks = new SortedDictionary<int, int>();
            int yearDiff = (end.Year - start.Year);
            int prefill_weeks =  yearDiff * 52;
            for(int i = 1; i <= prefill_weeks; i++)
            {
                pathoCountperWeeks.Add(i, 0);
            }

            foreach(JObject obj in result)
            {
                int week = this.GetIso8601WeekOfYear(Convert.ToDateTime(obj.Property("Zeitpunkt").Value.ToString()));
                pathoCountperWeeks[week] += 1;
            }

            int startweek = this.GetIso8601WeekOfYear(start);
            int weekcount = this.GetIso8601WeekOfYear(Convert.ToDateTime(endtime));

            if (yearDiff == 1)
            {
                weekcount += 53;
            }
            else if (yearDiff > 1)
            {
                weekcount += prefill_weeks + 1;
            }

            string weeks = "";
            string observation = "";

            JObject retObj = new JObject();
            retObj.Add("Startjahr", start.Year);
            retObj.Add("Startwoche", startweek);

            for(int i = startweek; i <= weekcount; i++)
            {
                weeks += i.ToString();
                observation += pathoCountperWeeks[i].ToString();

                if (i < weekcount)
                { 
                    weeks += ",";
                    observation += ",";
                }
            }

            retObj.Add("Woche", weeks);
            retObj.Add("Beobachtungen", observation);

            retArr.Add(retObj);

            return retArr;

        }
        #endregion

        #region HelperFunction

        private string printDateTime(DateTime dt)
        {
            return dt.ToString("s") + dt.ToString("zzz");
        }

        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        public int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        #endregion*/
    }

}