using ExcelDataReader;
using Newtonsoft.Json;
using SmICSCoreLib.JSONFileStream;
using SmICSCoreLib.StatistikDataModels;
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;

namespace SmICSCoreLib.StatistikServices
{
    public class RkiService
    {
        private readonly RestClient _client = new();
        private readonly ILogger<RkiService> _logger;
        public RkiService(ILogger<RkiService> logger)
        {
            _logger = logger;
        }


        //Get Data From RKI REST API
        public StateData GetStateData(int blId)
        {
            try
            {
                _client.EndPoint = "https://services7.arcgis.com/mOBPykOjAyBO2ZKk/arcgis/rest/services/rki_key_data_hubv/FeatureServer/0/query?where=AdmUnitId ='" + blId +
                "'&outFields=AnzFall,AnzTodesfall,AnzFallNeu,AnzTodesfallNeu,Inz7T&outSR=4326&f=json";
                string response = _client.GetResponse();
                var obj = JsonConvert.DeserializeObject<StateData>(response);

                _logger.LogInformation("GetStateData");
                return obj;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetStateData " + e.Message);
                return null;
            }
        }

        public State GetAllStates()
        {
            try
            {
                _client.EndPoint = "https://services7.arcgis.com/mOBPykOjAyBO2ZKk/arcgis/rest/services/Coronaf%C3%A4lle_in_den_Bundesl%C3%A4ndern/FeatureServer/0/query?" +
                "where=1=1&outFields=OBJECTID_1, LAN_ew_GEN, LAN_ew_BEZ, LAN_ew_EWZ, Fallzahl, faelle_100000_EW, Death, cases7_bl_per_100k, Aktualisierung&returnGeometry=false&outSR=4326&f=json";
                string response = _client.GetResponse();
                var obj = JsonConvert.DeserializeObject<State>(response);

                _logger.LogInformation("GetAllStates");
                return obj;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetAllStates " + e.Message);
                return null;
            }
        }

        public State GetStateByName(string bl)
        {
            try
            {
                _client.EndPoint = "https://services7.arcgis.com/mOBPykOjAyBO2ZKk/arcgis/rest/services/Coronaf%C3%A4lle_in_den_Bundesl%C3%A4ndern/FeatureServer/0/query?where=LAN_ew_GEN='" +
                bl + "'&outFields=OBJECTID_1, LAN_ew_GEN, LAN_ew_BEZ, LAN_ew_EWZ, Fallzahl,cases7_bl, faelle_100000_EW, Death, death7_bl, cases7_bl_per_100k, Aktualisierung&returnGeometry=false&outSR=4326&f=json";
                string response = _client.GetResponse();
                var obj = JsonConvert.DeserializeObject<State>(response);

                _logger.LogInformation("GetStateByName");
                return obj;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetStateByName " + e.Message);
                return null;
            }
        }

        public District GetDistrictsByStateName(string bl)
        {
            try
            {
                _client.EndPoint = "https://services7.arcgis.com/mOBPykOjAyBO2ZKk/arcgis/rest/services/RKI_Landkreisdaten/FeatureServer/0/query?where=BL='" +
                bl + "'&outFields=*&outSR=4326&f=json&returnGeometry=false";
                string response = _client.GetResponse();
                var obj = JsonConvert.DeserializeObject<District>(response);

                _logger.LogInformation("GetDistrictsByStateName");
                return obj;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetDistrictsByStateName " + e.Message);
                return null;
            }
        }

        public District GetDcistrictByName(string gen)
        {
            try
            {
                _client.EndPoint = "https://services7.arcgis.com/mOBPykOjAyBO2ZKk/arcgis/rest/services/RKI_Landkreisdaten/FeatureServer/0/query?where=GEN='" +
                gen + "'&outFields=*&outSR=4326&f=json&returnGeometry=false";
                string response = _client.GetResponse();
                var obj = JsonConvert.DeserializeObject<District>(response);

                _logger.LogInformation("GetDcistrictByName");
                return obj;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetDcistrictByName " + e.Message);
                return null;
            }
        }

        public DataSet GetDataSetFromLink(String url)
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

                _logger.LogInformation("GetDataSetFromLink");
                return result;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetDataSetFromLink " + e.Message);
                return null;
            }
        }

        public DataSet GetCsvDataSet(String url)
        {
            var client = new WebClient();
            try
            {
                var fullPath = Path.GetTempFileName();
                client.DownloadFile(url, fullPath);

                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using var stream = File.Open(fullPath, FileMode.Open, FileAccess.Read);
                using var reader = ExcelReaderFactory.CreateCsvReader(stream);
                var result = reader.AsDataSet();

                _logger.LogInformation("GetCsvDataSet");
                return result;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetCsvDataSet " + e.Message);
                return null;
            }
        }


        //Get Data From RKI Resources and write it as a DailyReport
        public string GetRValue(int vlaue)
        {
            string rValu;
            try
            {
                string url = "https://raw.githubusercontent.com/robert-koch-institut/SARS-CoV-2-Nowcasting_und_-R-Schaetzung/main/Nowcast_R_aktuell.csv";
                var result = GetCsvDataSet(url);
                var dataRows = result.Tables[0].Rows;
                rValu = result.Tables[0].Rows[dataRows.Count - vlaue][9].ToString();
                if (rValu.ElementAt(0) == '.')
                {
                    rValu = rValu.Insert(0, "0");
                }

                _logger.LogInformation("GetRValue");
                return rValu;
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetRValue " + e.Message);
                return null;
            }
        }

        public Bericht GetBerichtFromUrl(string url)
        {
            Bericht bericht = new();
            var result = GetDataSetFromLink(url);

            if (result != null)
            {
                try
                {
                    ArrayList bundeslaender = new();
                    ArrayList landkreise = new();
                    for (int i = 0; i < 16; i++)
                    {
                        Bundesland bundesland = new();
                        BlAttribute attr = new();
                        string[] bundes = new string[] { "Baden-Württemberg", "Bayern", "Berlin","Brandenburg", "Bremen", "Hamburg",
                        "Hessen", "Mecklenburg-Vorpommern", "Niedersachsen", "Nordrhein-Westfalen", "Rheinland-Pfalz", "Saarland",
                        "Sachsen", "Sachsen-Anhalt", "Schleswig-Holstein", "Thüringen"};

                        attr.Bundesland = bundes[i];
                        try
                        {
                            State state = GetStateByName(attr.Bundesland);
                            if (state.Features != null)
                            {
                                attr.FallzahlGesamt = state.Features[0].Attributes.Fallzahl.ToString("#,##");
                                attr.Faelle7BL = state.Features[0].Attributes.Cases7_bl.ToString("#,##");
                                attr.FaellePro100000Ew = state.Features[0].Attributes.FaellePro100000Ew.ToString("#,##");
                                attr.Todesfaelle = state.Features[0].Attributes.Todesfaelle.ToString("#,##");
                                attr.Todesfaelle7BL = state.Features[0].Attributes.Death7_bl.ToString("0.##");
                                attr.Inzidenz7Tage = (state.Features[0].Attributes.Faelle7BlPro100K).ToString("0.##").Replace(",", ".");
                                attr.Farbe = SetMapColor(attr.Inzidenz7Tage);
                                District district = GetDistrictsByStateName(attr.Bundesland);
                                if (district.Features != null && district.Features.Length != 0)
                                {
                                    landkreise = new ArrayList();
                                    foreach (var lk in district.Features)
                                    {
                                        Landkreis landkreisObj = new();
                                        landkreisObj.LandkreisName = lk.DistrictAttributes.County;
                                        landkreisObj.Stadt = lk.DistrictAttributes.GEN;
                                        landkreisObj.FallzahlGesamt = lk.DistrictAttributes.Cases.ToString("#,##");
                                        landkreisObj.Faelle7Lk = lk.DistrictAttributes.Cases7_lk.ToString("#,##");
                                        landkreisObj.FaellePro100000Ew = lk.DistrictAttributes.Cases_per_100k.ToString("#,##");
                                        landkreisObj.Inzidenz7Tage = lk.DistrictAttributes.Cases7_per_100k.ToString("0.##").Replace(",", ".");
                                        landkreisObj.Todesfaelle = lk.DistrictAttributes.Deaths.ToString("#,##");
                                        landkreisObj.Todesfaelle7Lk = lk.DistrictAttributes.Death7_lk.ToString("0.##");
                                        landkreisObj.AdmUnitId = lk.DistrictAttributes.AdmUnitId;
                                        landkreise.Add(landkreisObj);
                                    }
                                }
                                bericht.BlStandAktuell = true;
                            }
                        }
                        catch (Exception)
                        {
                            bericht.BlStandAktuell = false;
                        }
                        Landkreis[] lkArray = (Landkreis[])landkreise.ToArray(typeof(Landkreis));
                        bundesland.Landkreise = lkArray;
                        bundesland.BlAttribute = attr;
                        bundeslaender.Add(bundesland);
                    }
                    Bundesland[] blArray = (Bundesland[])bundeslaender.ToArray(typeof(Bundesland));
                    bericht.Bundesland = blArray;

                    StateData stateData = GetStateData(0);
                    if (stateData != null)
                    {
                        try
                        {
                            bericht.Fallzahl = stateData.DataFeature[0].DataAttributes.AnzFall.ToString("#,##");
                            bericht.FallzahlVortag = stateData.DataFeature[0].DataAttributes.AnzFallNeu.ToString("#,##");
                            bericht.Todesfaelle = stateData.DataFeature[0].DataAttributes.AnzTodesfall.ToString("#,##");
                            bericht.TodesfaelleVortag = stateData.DataFeature[0].DataAttributes.AnzTodesfallNeu.ToString("#,##");
                            bericht.Inzidenz7Tage = stateData.DataFeature[0].DataAttributes.Inz7T.ToString();
                            bericht.Stand = DateTime.Now.Date.ToString("dd.MM.yyyy");
                            string wert = GetRValue(2);
                            if (wert == null)
                            {
                                bericht.RWert7Tage = ("k.A.");
                                bericht.RWert7TageVortag = ("k.A.");
                            }
                            else
                            {
                                bericht.RWert7Tage = GetRValue(2).Replace(",", ".");
                                bericht.RWert7TageVortag = GetRValue(3).Replace(",", ".");
                            }
                        }
                        catch (Exception)
                        {
                            bericht.StandAktuell = true;
                            return bericht;
                        }
                    }

                    String urlImpfung = "https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Impfquotenmonitoring.xlsx?__blob=publicationFile";
                    var resultImpfung = GetDataSetFromLink(urlImpfung);
                    if (resultImpfung != null)
                    {
                        try
                        {
                            bericht.GesamtImpfung = Convert.ToDouble(resultImpfung.Tables[1].Rows[20][2]).ToString("#,##");
                            bericht.ErstImpfung = resultImpfung.Tables[1].Rows[20][6].ToString();
                            bericht.ZweitImpfung = resultImpfung.Tables[1].Rows[20][11].ToString();
                            bericht.ImpfStatus = true;
                        }
                        catch (Exception)
                        {
                            _logger.LogWarning("ImpfStatus is false");
                            bericht.ImpfStatus = false;
                        }
                    }

                    bericht.StandAktuell = true;
                    _logger.LogInformation("GetBerichtFromUrl");
                    return bericht;
                }
                catch (Exception e)
                {
                    _logger.LogWarning("GetBerichtFromUrl " + e.Message);
                    bericht.StandAktuell = false;
                    return null;
                }
            }
            else
            {
                _logger.LogWarning("GetDataSetFromLink Result is null");
                return null;
            }
        }

        public bool SerializeRkiData(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string filename = DateTime.Now.ToString("yyyy-MM-dd");
                string filePath = path + "/" + filename + ".json";
                bool status = false;
                string url = "https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Fallzahlen_Kum_Tab.xlsx?__blob=publicationFile";
                if (!File.Exists(filePath))
                {
                    DailyReport dailyReport = new();
                    Bericht bericht = GetBerichtFromUrl(url);
                    if (bericht != null)
                    {
                        dailyReport.Bericht = bericht;
                        JSONWriter.Write(dailyReport, path, filename);
                        status = true;
                    }
                    else
                    {
                        _logger.LogWarning("GetBerichtFromUrl Result is null");
                        status = false;
                    }
                }
                else
                {
                    try
                    {
                        DailyReport lastReport = DeserializeRkiData(filePath);
                        bool standAktuell = lastReport.Bericht.StandAktuell;
                        string stand = lastReport.Bericht.Stand.Substring(0, 10);
                        string date = DateTime.Now.ToString("dd.MM.yyyy");

                        if (standAktuell == false)
                        {
                            DailyReport dailyReport = new();
                            Bericht bericht = GetBerichtFromUrl(url);
                            if (bericht != null)
                            {
                                dailyReport.Bericht = bericht;
                                JSONWriter.Write(dailyReport, path, filename);
                                status = true;
                            }
                            else
                            {
                                _logger.LogWarning("GetBerichtFromUrl Result is null");
                                status = false;
                            }
                        }
                        else if (stand != date)
                        {
                            DailyReport dailyReport = new();
                            Bericht bericht = GetBerichtFromUrl(url);
                            dailyReport.Bericht = bericht;
                            JSONWriter.Write(dailyReport, path, filename);
                            status = true;
                        }
                        else
                        {
                            status = true;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning("SerializeRkiData " + e.Message);
                        status = false;
                    }
                }

                _logger.LogInformation("SerializeRkiData");
                return status;
            }
            catch (Exception e)
            {
                _logger.LogWarning("SerializeRkiData " + e.Message);
                return false;
            }
        }

        public DailyReport DeserializeRkiData(string path)
        {
            try
            {
                DailyReport dailyReport = JSONReader<DailyReport>.ReadObject(path);
                _logger.LogInformation("DeserializeRkiData");
                return dailyReport;
            }
            catch (Exception e)
            {
                _logger.LogWarning("DeserializeRkiData " + e.Message);
                return null;
            }
        }

        public string SetMapColor(string inzidenz)
        {
            string farbe;
            try
            {
                if (inzidenz.Contains("."))
                {
                    int index = inzidenz.IndexOf(".");
                    if (index == 0)
                    {
                        inzidenz = "0";
                    }
                    else
                    {
                        inzidenz = inzidenz.Substring(0, index);
                    }
                }
                int zahl = (int)Convert.ToInt64(Math.Floor(Convert.ToDouble(inzidenz)));

                if (zahl >= 100)
                {
                    farbe = "#671212";
                    return farbe;
                }
                if (zahl < 100 && zahl >= 75)
                {
                    farbe = "#951214";
                    return farbe;
                }
                if (zahl < 75 && zahl >= 50)
                {
                    farbe = "#D43624";
                    return farbe;
                }
                if (zahl < 50 && zahl >= 25)
                {
                    farbe = "#FFB534";
                    return farbe;
                }
                if (zahl < 25 && zahl >= 5)
                {
                    farbe = "#FFF380";
                    return farbe;
                }
                if (zahl < 5 && zahl > 0)
                {
                    farbe = "#FFFCCD";
                    return farbe;
                }
                else
                {
                    farbe = "#FFFFFF";
                    return farbe;
                }
                _logger.LogInformation("SetMapColor");
            }
            catch (Exception e)
            {
                _logger.LogWarning("SetMapColor " + e.Message);
                farbe = "#FFFFFF";
                return farbe;
            }
        }

        public string SetCaseColor(string tag, string vortag)
        {
            string color;
            if (tag != null && vortag != null)
            {
                try
                {
                    tag = tag.Replace(".", "").Trim();
                    vortag = vortag.Replace(".", "").Trim();
                    double tagToDouble = double.Parse(tag);
                    double vortagToDouble = double.Parse(vortag);

                    if (tagToDouble < vortagToDouble)
                    {
                        color = "#66C166";
                        return color;
                    }
                    if (tagToDouble == vortagToDouble)
                    {
                        color = "#FFC037";
                        return color;
                    }
                    if (tagToDouble > vortagToDouble)
                    {
                        color = "#F35C58";
                        return color;
                    }
                    else
                    {
                        color = "#8CA2AE";
                        return color;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogWarning("SetCaseColor " + e.Message);
                    color = "#b0bec5";
                    return color;
                }
            }
            else
            {
                color = "#b0bec5";
                return color;
            }
        }


        //Get RKI Data from Excel to Json
        public Report GetBLReport(string url, int tabelle, int zeileDatum, int zeileFahlahl, int spalte, int laenge)
        {
            try
            {
                Report report = new();
                var result = GetDataSetFromLink(url);
                if (result != null)
                {
                    ArrayList reportArrayList = new();

                    for (int i = zeileFahlahl; i < 19; i++)
                    {
                        BLReport blReportObj = new();
                        blReportObj.BlName = result.Tables[tabelle].Rows[i][0].ToString();
                        ArrayList blReportArrayList = new();
                        for (int y = spalte; y < laenge; y++)
                        {
                            BLReportAttribute bLReportObj = new();
                            bLReportObj.Datum = result.Tables[tabelle].Rows[zeileDatum][y].ToString().Substring(0, 10);
                            try
                            {
                                bLReportObj.Fahlzahl = int.Parse(result.Tables[tabelle].Rows[i][y].ToString());
                            }
                            catch (Exception)
                            {
                                bLReportObj.Fahlzahl = 0;
                            }
                            blReportArrayList.Add(bLReportObj);
                        }
                        BLReportAttribute[] blReportAttributeArray = (BLReportAttribute[])blReportArrayList.ToArray(typeof(BLReportAttribute));
                        blReportObj.BLReportAttribute = blReportAttributeArray;
                        reportArrayList.Add(blReportObj);
                    }
                    BLReport[] blReportArray = (BLReport[])reportArrayList.ToArray(typeof(BLReport));
                    report.Datum = DateTime.Now.ToString("dd.MM.yyyy");
                    report.BLReport = blReportArray;

                    _logger.LogInformation("GetBLReport");
                    return report;
                }
                else
                {
                    _logger.LogWarning("GetBLReport Result is null ");
                    return null;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetBLReport " + e.Message);
                return null;
            }       
        }

        public LKReportJson GetLKReport(string url, int tabelle, int zeileDatum, int zeileFahlahl, int spalte, int laenge)
        {
            try
            {
                LKReportJson lkReportJson = new();

                var result = GetDataSetFromLink(url);
                if (result != null)
                {
                    ArrayList reportArrayList = new();

                    for (int i = zeileFahlahl; i <= 416; i++)
                    {
                        LKReport lkReportObj = new();
                        lkReportObj.LKName = result.Tables[tabelle].Rows[i][1].ToString();
                        lkReportObj.AdmUnitId = int.Parse(result.Tables[tabelle].Rows[i][2].ToString());
                        ArrayList blReportArrayList = new();
                        for (int y = spalte; y < laenge; y++)
                        {
                            LKReportAttribute reportAttribute = new();
                            reportAttribute.Datum = result.Tables[tabelle].Rows[zeileDatum][y].ToString().Substring(0, 10);
                            try
                            {
                                reportAttribute.Fahlzahl = int.Parse(result.Tables[tabelle].Rows[i][y].ToString());
                            }
                            catch (Exception)
                            {
                                reportAttribute.Fahlzahl = 0;
                            }
                            blReportArrayList.Add(reportAttribute);
                        }
                        LKReportAttribute[] lkReportAttributeArray = (LKReportAttribute[])blReportArrayList.ToArray(typeof(LKReportAttribute));
                        lkReportObj.LKReportAttribute = lkReportAttributeArray;
                        reportArrayList.Add(lkReportObj);
                    }
                    LKReport[] blReportArray = (LKReport[])reportArrayList.ToArray(typeof(LKReport));
                    lkReportJson.Datum = DateTime.Now.ToString("dd.MM.yyyy");
                    lkReportJson.LKReport = blReportArray;

                    _logger.LogInformation("GetLKReport");
                    return lkReportJson;
                }
                else
                {
                    _logger.LogWarning("GetLKReport Result is null");
                    return null;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning("GetLKReport " + e.Message);
                return null;
            }
        }

        public bool BLReportSerialize(string path)
        {
            Report report = GetBLReport("https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Fallzahlen_Kum_Tab.xlsx?__blob=publicationFile", 2, 2, 3, 1, 457);
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filename = ("BLReport");
                JSONWriter.Write(report, path, filename);

                _logger.LogInformation("BLReportSerialize");
                return true;
            }
            catch (Exception e)
            {
                _logger.LogWarning("BLReportSerialize " + e.Message);
                return false;
            }
        }

        public bool LKReportSerialize(string path)
        {
            LKReportJson lKReportJson = GetLKReport("https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Fallzahlen_Kum_Tab.xlsx?__blob=publicationFile", 4, 4, 5, 3, 263);
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filename = ("LKReport");
                JSONWriter.Write(lKReportJson, path, filename);

                _logger.LogInformation("LKReportSerialize");
                return true;
            }
            catch (Exception e)
            {
                _logger.LogWarning("LKReportSerialize " + e.Message);
                return false;
            }
        }

        //Update RKI Data with a CronJob
        public Report BLReportDeserialize(string filePath)
        {
            try
            {
                Report blReportJson = JSONReader<Report>.ReadObject(filePath);
                _logger.LogInformation("BLReportDeserialize");
                return blReportJson;
            }
            catch (Exception e)
            {
                _logger.LogWarning("BLReportDeserialize " + e.Message);
                return null;
            }
        }

        public LKReportJson LKReportDeserialize(string filePath)
        {
            try
            {
                LKReportJson lKReportJson = JSONReader<LKReportJson>.ReadObject(filePath);
                _logger.LogInformation("LKReportDeserialize");
                return lKReportJson;
            }
            catch (Exception e)
            {
                _logger.LogWarning("LKReportDeserialize " + e.Message);
                return null;
            }
        }

        public bool UpdateBlRkidata(string dailyReportPath, string blReportPath, string targetPath, string filename)
        {
            try
            {
                DailyReport dailyReport = DeserializeRkiData(dailyReportPath);
                if (dailyReport != null)
                {
                    Bundesland[] bundeslaender = dailyReport.Bericht.Bundesland;
                    Report report = new();
                    BLReportAttribute[] bLAttributeObj;
                    ArrayList reportArrayList = new();
                    ArrayList blAttributeArrayList = new();
                    BLReportAttribute[] blReportAttributeArray;

                    Report blReportJson = BLReportDeserialize(blReportPath);
                    if (blReportJson != null)
                    {
                        int count = 0;
                        foreach (var bl in blReportJson.BLReport)
                        {
                            BLReport blReport = new();
                            BLReportAttribute bLReportAttributeObj = new();
                            bLAttributeObj = bl.BLReportAttribute;
                            blAttributeArrayList = new ArrayList(bLAttributeObj);

                            bLReportAttributeObj.Datum = DateTime.Now.ToString("dd.MM.yyyy");
                            bLReportAttributeObj.Fahlzahl = (int)double.Parse(bundeslaender[count].BlAttribute.Faelle7BL);

                            blAttributeArrayList.Add(bLReportAttributeObj);
                            blReportAttributeArray = (BLReportAttribute[])blAttributeArrayList.ToArray(typeof(BLReportAttribute));
                            blReport.BLReportAttribute = blReportAttributeArray;
                            blReport.BlName = bl.BlName;
                            reportArrayList.Add(blReport);
                            count++;
                        }

                        BLReport[] blReportArray = (BLReport[])reportArrayList.ToArray(typeof(BLReport));
                        report.BLReport = blReportArray;
                        report.Datum = DateTime.Now.ToString("dd.MM.yyyy");

                        JSONWriter.Write(report, targetPath, filename);

                        _logger.LogInformation("UpdateBlRkidata");
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("UpdateBlRkidata Result is null ");
                        return false;
                    }
                }
                else
                {
                    _logger.LogWarning("UpdateBlRkidata Result is null");
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning("UpdateBlRkidata " + e.Message);
                return false;
            }
        }

        public bool UpdateLklRkidata(string dailyReportPath, string lkReportPath, string targetPath, string filename)
        {
            try
            {
                DailyReport dailyReport = DeserializeRkiData(dailyReportPath);
                if (dailyReport != null)
                {
                    Bundesland[] bundeslaender = dailyReport.Bericht.Bundesland;
                    ArrayList allLkArrayList = new();

                    foreach (var blItem in bundeslaender)
                    {
                        Landkreis[] lk = blItem.Landkreise;

                        foreach (var lkItem in lk)
                        {
                            allLkArrayList.Add(lkItem);
                        }
                    }
                    Landkreis[] landkreise = (Landkreis[])allLkArrayList.ToArray(typeof(Landkreis));
                    Landkreis[] landkreiseSort = landkreise.OrderBy(c => c.LandkreisName).ToArray();

                    LKReportJson report = new();
                    LKReportAttribute[] lkAttributeObj;
                    ArrayList reportArrayList = new();
                    ArrayList lkAttributeArrayList = new();
                    LKReportAttribute[] lkReportAttributeArray;
                    LKReportJson lklReportJson = LKReportDeserialize(lkReportPath);
                    if (lklReportJson != null)
                    {
                        foreach (var lk in lklReportJson.LKReport)
                        {
                            foreach (var landkreis in landkreiseSort)
                            {
                                if (lk.AdmUnitId == landkreis.AdmUnitId)
                                {
                                    LKReport lkReport = new();
                                    LKReportAttribute lkReportAttributeObj = new();
                                    lkAttributeObj = lk.LKReportAttribute;
                                    lkAttributeArrayList = new ArrayList(lkAttributeObj);

                                    lkReportAttributeObj.Datum = DateTime.Now.ToString("dd.MM.yyyy");
                                    try
                                    {
                                        lkReportAttributeObj.Fahlzahl = (int)double.Parse(landkreis.Faelle7Lk);
                                    }
                                    catch
                                    {
                                        lkReportAttributeObj.Fahlzahl = 0;
                                    }

                                    lkAttributeArrayList.Add(lkReportAttributeObj);
                                    lkReportAttributeArray = (LKReportAttribute[])lkAttributeArrayList.ToArray(typeof(LKReportAttribute));
                                    lkReport.LKReportAttribute = lkReportAttributeArray;

                                    if (lkReport.AdmUnitId == 5358)
                                    {
                                        lkReport.LKName = lk.LKName;
                                        lkReport.AdmUnitId = lk.AdmUnitId;
                                    }
                                    lkReport.LKName = lk.LKName;
                                    lkReport.AdmUnitId = lk.AdmUnitId;
                                    reportArrayList.Add(lkReport);
                                }
                            }
                        }
                        LKReport[] lkReportArray = (LKReport[])reportArrayList.ToArray(typeof(LKReport));
                        report.LKReport = lkReportArray;
                        report.Datum = DateTime.Now.ToString("dd.MM.yyyy");

                        JSONWriter.Write(report, targetPath, filename);

                        _logger.LogInformation("UpdateLklRkidata");
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("UpdateLklRkidata Result is null");
                        return false;
                    }
                }
                else
                {
                    _logger.LogWarning("UpdateLklRkidata Result is null");
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning("UpdateLklRkidata " + e.Message);
                return false;
            }
        }
    }
}
