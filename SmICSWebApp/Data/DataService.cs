using SmICSWebApp.Helper;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using SmICSCoreLib.AQL.Patient_Stay;
using SmICSCoreLib.AQL.Patient_Stay.Stationary;
using SmICSCoreLib.AQL.Patient_Stay.Count;
using System.IO;
using ExcelDataReader;
using System.Net;
using System.Globalization;
using System.Data;
using SmICSCoreLib.JSONFileStream;
using System.Collections;
using System.Linq;
using SmICSCoreLib.AQL.PatientInformation;
using SmICSCoreLib.AQL.PatientInformation.Symptome;
using SmICSCoreLib.AQL.PatientInformation.PatientMovement;
using SmICSCoreLib.AQL.General;

namespace SmICSWebApp.Data
{
    public class DataService
    {
        private readonly RestClient client = new RestClient();
        private readonly IPatinet_Stay _patinet_Stay;
        private readonly IPatientInformation _patientInformation;

        public DataService(IPatinet_Stay patinet_Stay, IPatientInformation patientInformation)
        {
            _patinet_Stay = patinet_Stay;
            _patientInformation = patientInformation;
        }

        //Load RkiData
        public State GetStates()
        {
            try
            {
                client.endPoint = "https://services7.arcgis.com/mOBPykOjAyBO2ZKk/arcgis/rest/services/Coronaf%C3%A4lle_in_den_Bundesl%C3%A4ndern/FeatureServer/0/query?" +
              "where=1=1&outFields=OBJECTID_1, LAN_ew_GEN, LAN_ew_BEZ, LAN_ew_EWZ, Fallzahl, faelle_100000_EW, Death, cases7_bl_per_100k, Aktualisierung&returnGeometry=false&outSR=4326&f=json";

                string response = client.GetResponse();
                var obj = JsonConvert.DeserializeObject<State>(response);

                return obj;
            }
            catch (Exception)
            {

                return null;
            }
        }
        public State GetState(string bl)
        {
            try
            {
                client.endPoint = "https://services7.arcgis.com/mOBPykOjAyBO2ZKk/arcgis/rest/services/Coronaf%C3%A4lle_in_den_Bundesl%C3%A4ndern/FeatureServer/0/query?where=LAN_ew_GEN='" +
                bl + "'&outFields=OBJECTID_1, LAN_ew_GEN, LAN_ew_BEZ, LAN_ew_EWZ, Fallzahl,cases7_bl, faelle_100000_EW, Death, death7_bl, cases7_bl_per_100k, Aktualisierung&returnGeometry=false&outSR=4326&f=json";

                string response = client.GetResponse();
                var obj = JsonConvert.DeserializeObject<State>(response);

                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public District GetDistricts(string bl)
        {
            try
            {
                client.endPoint = "https://services7.arcgis.com/mOBPykOjAyBO2ZKk/arcgis/rest/services/RKI_Landkreisdaten/FeatureServer/0/query?where=BL='" +
                    bl + "'&outFields=*&outSR=4326&f=json&returnGeometry=false";
                string response = client.GetResponse();
                var obj = JsonConvert.DeserializeObject<District>(response);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public District GetDistrict(string gen)
        {
            try
            {
                client.endPoint = "https://services7.arcgis.com/mOBPykOjAyBO2ZKk/arcgis/rest/services/RKI_Landkreisdaten/FeatureServer/0/query?where=GEN='" +
                gen + "'&outFields=*&outSR=4326&f=json&returnGeometry=false";
                string response = client.GetResponse();
                var obj = JsonConvert.DeserializeObject<District>(response);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Load EhrData
        public List<StationaryDataModel> GetStationaryPat(string patientID, string fallkennung, DateTime datum)
        {
            List<StationaryDataModel> stationaryDatas = _patinet_Stay.Stationary_Stay(patientID, fallkennung, datum);
            return stationaryDatas;
        }
        public List<PatientMovementModel> GetPatMovement(string patientId)
        {
            List<string> patientList = new();
            patientList.Add(patientId);
            PatientListParameter patListParameter = new();
            patListParameter.patientList = patientList;
            List<PatientMovementModel> patientMovement = _patientInformation.Patient_Bewegung_Ps(patListParameter);
            return patientMovement;
        }

        public List<PatientMovementModel> GetPatMovementFromStation(List<string> patientList, string station, DateTime starttime, DateTime endtime)
        {
            //List<string> patientList = new();
            //patientList.Add(patientId);
            PatientListParameter patListParameter = new();
            patListParameter.patientList = patientList;
            List<PatientMovementModel> patientMovement = _patientInformation.Patient_Bewegung_Station(patListParameter, station, starttime, endtime);
            return patientMovement;
        }

        public List<CountDataModel> GetCovidPat(string nachweis)
        {
            List<CountDataModel> covidPat = _patinet_Stay.CovidPat(nachweis);
            return covidPat;
        }
        public List<CountDataModel> GetAllPositivTest()
        {
            List<CountDataModel> allPositivTest = GetCovidPat("260373001");
            return allPositivTest;
        }
        public List<CountDataModel> GetPositivPat(List<CountDataModel> allPositivPat)
        {
            List<CountDataModel> positivPat = new List<CountDataModel>();
            foreach (CountDataModel countData in allPositivPat)
            {
                if (!positivPat.Contains(countData))
                {
                    positivPat.Add(countData);
                }
                else
                {
                    CountDataModel data = positivPat.Find(i => i.PatientID == countData.PatientID);

                    if (data.Zeitpunkt_des_Probeneingangs > countData.Zeitpunkt_des_Probeneingangs)
                    {
                        positivPat.Remove(data);
                        positivPat.Add(countData);
                    }
                }
            }
            return positivPat;
        }
        public List<CountDataModel> GetNegativPat()
        {
            List<CountDataModel> allNegativPat = GetCovidPat("260415000");
            return allNegativPat;
        }
        public List<SymptomModel> GetAllSymByPat(string patientId, DateTime datum)
        {
            List<SymptomModel> symptomListe = _patientInformation.Symptoms_By_PatientId(patientId, datum);
            return symptomListe;
        }
        public List<Patient> GetAllNoskumalPat(List<CountDataModel> positivPatList)
        {
            List<Patient> patNoskumalList = new();
            List<string> symptomList = new List<string>(new string[] { "Chill (finding)", "Cough (finding)", "Dry cough (finding)",
            "Diarrhea (finding)", "Fever (finding)", "Fever greater than 100.4 Fahrenheit", "38° Celsius (finding)", "Nausea (finding)",
            "Pain in throat (finding)"});

            foreach (CountDataModel positivPat in positivPatList)
            {
                List<StationaryDataModel> statPatList = GetStationaryPat(positivPat.PatientID, positivPat.Fallkennung, positivPat.Zeitpunkt_des_Probeneingangs);

                if (statPatList != null || statPatList.Count != 0)
                {
                    foreach (StationaryDataModel statPatient in statPatList)
                    {
                        List<SymptomModel> symptoms = GetAllSymByPat(statPatient.PatientID, statPatient.Datum_Uhrzeit_der_Aufnahme);
                        if (symptoms is null || symptoms.Count == 0)
                        {
                            patNoskumalList.Add(new Patient(positivPat.PatientID, positivPat.Zeitpunkt_des_Probeneingangs, statPatient.Datum_Uhrzeit_der_Aufnahme, statPatient.Datum_Uhrzeit_der_Entlassung));
                        }
                        else
                        {
                            foreach (var symptom in symptoms)
                            {
                                if (!symptomList.Contains(symptom.NameDesSymptoms))
                                {
                                    patNoskumalList.Add(new Patient(positivPat.PatientID, positivPat.Zeitpunkt_des_Probeneingangs, statPatient.Datum_Uhrzeit_der_Aufnahme, statPatient.Datum_Uhrzeit_der_Entlassung));
                                }
                            }
                        }

                    }
                }
            }
            return patNoskumalList;
        }
        public List<Patient> GetNoskumalByContact(List<Patient> allNoskumalPat, List<CountDataModel> allPositivPat)
        {
            List<Patient> patNoskumalList = new ();

            foreach (var patient in allNoskumalPat)
            {
                List<PatientMovementModel> patBewegungen = GetPatMovement(patient.PatientID);
                if (patBewegungen.Count != 0)
                {
                    foreach (var bewegung in patBewegungen)
                    {
                        if (bewegung.Beginn < bewegung.Ende.AddMinutes(-15))
                        {
                            List <PatientMovementModel> patientMovement = FindContact(allPositivPat, bewegung.PatientID, 
                                bewegung.Fachabteilung, bewegung.Beginn, bewegung.Ende);

                            if (patientMovement.Count != 0)
                            {
                                patNoskumalList.Add(patient);
                            }
                        }
                    }
                }
            }
            return patNoskumalList;
        }
        public List<PatientMovementModel> FindContact(List<CountDataModel> allPositivPat, string patientID, string station, DateTime beginn, DateTime ende  ) {

            List <PatientMovementModel> patientMovement = new();
            List<string> patientList = new();

            foreach (var positivPat in allPositivPat)
            {
                patientList.Add(positivPat.PatientID);
            }

            List<PatientMovementModel> patBewegungen = GetPatMovementFromStation(patientList, station, beginn, ende);
            if (patBewegungen.Count != 0)
            {
                foreach (var patBewegung in patBewegungen)
                {
                    if (patBewegung.PatientID != patientID &&
                        patBewegung.Beginn < patBewegung.Ende.AddMinutes(-15))
                    {
                        patientMovement.Add(patBewegung);
                    }
                }    
            }            
            return patientMovement;
        }

        //
        public int PatStay(List<CountDataModel> positivPat)
        {
            double start;
            double gesamt = 0;
            foreach (CountDataModel item in positivPat)
            {
                List<StationaryDataModel> statPatList = _patinet_Stay.StayFromCase(item.PatientID, item.Fallkennung);

                foreach (StationaryDataModel statData in statPatList)
                {
                    start = (statData.Datum_Uhrzeit_der_Entlassung - statData.Datum_Uhrzeit_der_Aufnahme).TotalDays;
                    gesamt += start;
                }
            }


            return Convert.ToInt32(gesamt);
        }
        public DataSet GetStringFromRki(String url)
        {
            var client = new WebClient();
            try
            {
                var fullPath = Path.GetTempFileName();
                client.DownloadFile(url, fullPath);

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using var stream = File.Open(fullPath, FileMode.Open, FileAccess.Read);
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var result = reader.AsDataSet();

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public string GetRValue(int vlaue)
        {
            string rValu;
            try
            {
                String url = "https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Projekte_RKI/Nowcasting_Zahlen.xlsx?__blob=publicationFile";
                var result = GetStringFromRki(url);
                var dataRows = result.Tables[1].Rows;
                return rValu = result.Tables[1].Rows[dataRows.Count - vlaue][10].ToString();
            }
            catch (Exception)
            {
                return null;
            }

        }
        public Bericht GetBericht(string url)
        {

            Bericht bericht = new();
            var result = GetStringFromRki(url);

            if (result != null)
            {
                ArrayList bundeslaender = new ArrayList();
                ArrayList landkreise = new ArrayList();
                for (int i = 5; i < 21; i++)
                {
                    Bundesland bundesland = new();
                    BlAttribute attr = new();

                    attr.bundesland = result.Tables[0].Rows[i][0].ToString();
                    try
                    {
                        State state = GetState(attr.bundesland);
                        if (state.Features != null)
                        {
                            attr.fallzahlGesamt = state.Features[0].Attributes.Fallzahl;
                            attr.faelle7BL = state.Features[0].Attributes.Cases7_bl;
                            attr.faellePro100000Ew = state.Features[0].Attributes.FaellePro100000Ew;
                            attr.todesfaelle = state.Features[0].Attributes.Todesfaelle;
                            attr.todesfaelle7BL = state.Features[0].Attributes.Death7_bl;
                            bericht.blStandAktuell = true;

                            District district = GetDistricts(attr.bundesland);
                            if (district.features != null)
                            {
                                landkreise = new ArrayList();
                                foreach (var lk in district.features)
                                {
                                    Landkreis landkreisObj = new();
                                    landkreisObj.landkreis = lk.districtAttributes.county;
                                    landkreisObj.stadt = lk.districtAttributes.GEN;
                                    landkreisObj.fallzahlGesamt = lk.districtAttributes.cases;
                                    landkreisObj.faelle7Lk = lk.districtAttributes.cases7_lk;
                                    landkreisObj.faellePro100000Ew = lk.districtAttributes.cases_per_100k;
                                    landkreisObj.inzidenz7Tage = (float)(Math.Round(lk.districtAttributes.cases7_per_100k, 2));
                                    landkreisObj.todesfaelle = lk.districtAttributes.deaths;
                                    landkreisObj.todesfaelle7Lk = lk.districtAttributes.death7_lk;

                                    landkreise.Add(landkreisObj);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        bericht.blStandAktuell = false;
                    }
                    attr.inzidenz7Tage = result.Tables[0].Rows[i][2].ToString().Substring(0, 5);
                    attr.farbe = SeMapColor(attr.inzidenz7Tage);
                    Landkreis[] lkArray = (Landkreis[])landkreise.ToArray(typeof(Landkreis));
                    bundesland.landkreis = lkArray;
                    bundesland.blAttribute = attr;
                    bundeslaender.Add(bundesland);
                }
                Bundesland[] blArray = (Bundesland[])bundeslaender.ToArray(typeof(Bundesland));
                bericht.bundesland = blArray;

                String urlImpfung = "https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Impfquotenmonitoring.xlsx?__blob=publicationFile";
                var resultImpfung = GetStringFromRki(urlImpfung);

                if (resultImpfung != null)
                {
                    try
                    {
                        bericht.gesamtImpfung = resultImpfung.Tables[1].Rows[21][2].ToString();
                        bericht.erstImpfung = resultImpfung.Tables[1].Rows[21][5].ToString().Substring(0, 4);
                        bericht.zweitImpfung = resultImpfung.Tables[1].Rows[21][8].ToString().Substring(0, 4);
                        bericht.ImpfStatus = true;
                    }
                    catch (Exception)
                    {
                        bericht.ImpfStatus = false;
                    }
                }
                
                bericht.stand = result.Tables[0].Rows[1][0].ToString().Substring(7);
                bericht.rWert7Tage = GetRValue(2);
                bericht.rWert7TageVortag = GetRValue(3);
                bericht.inzidenz7Tage = result.Tables[0].Rows[21][2].ToString().Substring(0, 5);
                bericht.inzidenz7TageVortag = result.Tables[0].Rows[21][2].ToString().Substring(0, 5);

                //TODO:Separate from Excel table 
                var dataRows = result.Tables[2].Rows;
                bericht.fallzahl = result.Tables[2].Rows[dataRows.Count - 1][1].ToString();
                bericht.fallzahlVortag = result.Tables[2].Rows[dataRows.Count - 1][3].ToString();
                bericht.todesfaelle = result.Tables[2].Rows[dataRows.Count - 1][4].ToString();
                bericht.todesfaelleVortag = result.Tables[2].Rows[dataRows.Count - 1][5].ToString();
                bericht.standAktuell = true;
            }
            else
            {
                bericht.standAktuell = false;
            }
            return bericht;
        }
        public void SerializeRkiData()
        {
            string path = @"../SmICSWebApp/Resources/statistik/json";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filename = DateTime.Now.ToString("yyyy-MM-dd");
            string filePath = path + "/" + filename + ".json";

            if (!File.Exists(filePath))
            {
                DailyReport dailyReport = new();
                Bericht bericht = GetBericht("https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Fallzahlen_Kum_Tab.xlsx?__blob=publicationFile");
                dailyReport.bericht = bericht;
                JSONWriter.Write(dailyReport, path, filename);
            }
            else
            {
                try
                {
                    DailyReport lastReport = DeserializeRkiData(DateTime.Now);
                    bool standAktuell = lastReport.bericht.standAktuell;
                    if (standAktuell == false)
                    {
                        DailyReport dailyReport = new();
                        Bericht bericht = GetBericht("");
                        dailyReport.bericht = bericht;
                        JSONWriter.Write(dailyReport, path, filename);
                    }
                    else
                    {
                        string stand = lastReport.bericht.stand.Substring(0, 10);
                        string date = DateTime.Now.ToString("dd.MM.yyyy");
                        if (stand != date)
                        {
                            DailyReport dailyReport = new();
                            Bericht bericht = GetBericht("https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Fallzahlen_Kum_Tab.xlsx?__blob=publicationFile");
                            dailyReport.bericht = bericht;
                            JSONWriter.Write(dailyReport, path, filename);
                        }
                    }
                }
                catch (Exception)
                {
                    DailyReport dailyReport = new();
                    Bericht bericht = GetBericht("https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Fallzahlen_Kum_Tab.xlsx?__blob=publicationFile");
                    dailyReport.bericht = bericht;
                    JSONWriter.Write(dailyReport, path, filename);
                }
            }
        }
        public DailyReport DeserializeRkiData(DateTime datum)
        {
            string path = @"../SmICSWebApp/Resources/statistik/json/" + datum.ToString("yyyy-MM-dd") + ".json";
            try
            {
                DailyReport dailyReport = JSONReader<DailyReport>.ReadObject(path);
                return dailyReport;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public string SeMapColor(string inzidenz)
        {
            string farbe;          
            int zahl = (int)Convert.ToInt64(Math.Floor(Convert.ToDouble(inzidenz)));

            if (zahl > 100)
            {
                farbe = "#671212";
                return farbe;
            }
            if (zahl < 100 && zahl > 75)
            {
                farbe = "#951214";
                return farbe;
            }
            if (zahl < 75 && zahl > 50)
            {
                farbe = "#D43624";
                return farbe;
            }
            if (zahl < 50 && zahl > 25)
            {
                farbe = "#FFB534";
                return farbe;
            }
            if (zahl < 25 && zahl > 5)
            {
                farbe = "#FFF380";
                return farbe;
            }
            else
            {
                farbe = "#FFFFFF";
                return farbe;
            }
        }
        public string SetCaseColor(double tag, double vortag)
        {
            string color;
            if (tag < vortag)
            {
                color = "#66C166";
                return color;
            }
            if (tag == vortag)
            {
                color = "#FFC037";
                return color;
            }
            if (tag > vortag)
            {
                color = "#F35C58";
                return color;
            }
            else
            {
                color = "#5591BB";
                return color;
            }
        }

    }
}
