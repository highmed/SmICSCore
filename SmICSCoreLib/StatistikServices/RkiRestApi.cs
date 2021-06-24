using ExcelDataReader;
using Newtonsoft.Json;
using SmICSCoreLib.JSONFileStream;
using SmICSCoreLib.StatistikDataModels;
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Net;

namespace SmICSCoreLib.StatistikServices
{
    public class RkiRestApi
    {
        private readonly RestClient client = new ();

        //Get Data From RKI REST API

        public StateData GetStateData(int blId)
        {
            try
            {
                client.EndPoint = "https://services7.arcgis.com/mOBPykOjAyBO2ZKk/arcgis/rest/services/rki_key_data_hubv/FeatureServer/0/query?where=AdmUnitId ='" + blId + "'&outFields=AnzFall,AnzTodesfall,AnzFallNeu,AnzTodesfallNeu,Inz7T&outSR=4326&f=json";

                string response = client.GetResponse();
                var obj = JsonConvert.DeserializeObject<StateData>(response);

                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public State GetAllStates()
        {
            try
            {
                client.EndPoint = "https://services7.arcgis.com/mOBPykOjAyBO2ZKk/arcgis/rest/services/Coronaf%C3%A4lle_in_den_Bundesl%C3%A4ndern/FeatureServer/0/query?" +
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

        public State GetStateByName(string bl)
        {
            try
            {
                client.EndPoint = "https://services7.arcgis.com/mOBPykOjAyBO2ZKk/arcgis/rest/services/Coronaf%C3%A4lle_in_den_Bundesl%C3%A4ndern/FeatureServer/0/query?where=LAN_ew_GEN='" +
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

        public District GetDistrictsByStateName(string bl)
        {
            try
            {
                client.EndPoint = "https://services7.arcgis.com/mOBPykOjAyBO2ZKk/arcgis/rest/services/RKI_Landkreisdaten/FeatureServer/0/query?where=BL='" +
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

        public District GetDistrictByName(string gen)
        {
            try
            {
                client.EndPoint = "https://services7.arcgis.com/mOBPykOjAyBO2ZKk/arcgis/rest/services/RKI_Landkreisdaten/FeatureServer/0/query?where=GEN='" +
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

        public static DataSet GetDataSetFromLink(String url)
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

        //Get Data From RKI Resources and write it as a DailyReport
        public string GetRValue(int vlaue)
        {
            string rValu;
            try
            {
                String url = "https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Projekte_RKI/Nowcasting_Zahlen.xlsx?__blob=publicationFile";
                var result = GetDataSetFromLink(url);
                var dataRows = result.Tables[1].Rows;
                return rValu = result.Tables[1].Rows[dataRows.Count - vlaue][10].ToString();
            }
            catch (Exception)
            {
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
                                bericht.BlStandAktuell = true;
                                attr.Farbe = SeMapColor(attr.Inzidenz7Tage);
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

                                        landkreise.Add(landkreisObj);
                                    }
                                }
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
                        bericht.Fallzahl = stateData.DataFeature[0].DataAttributes.AnzFall.ToString("#,##");
                        bericht.FallzahlVortag = stateData.DataFeature[0].DataAttributes.AnzFallNeu.ToString("#,##");
                        bericht.Todesfaelle = stateData.DataFeature[0].DataAttributes.AnzTodesfall.ToString("#,##");
                        bericht.TodesfaelleVortag = stateData.DataFeature[0].DataAttributes.AnzTodesfallNeu.ToString("#,##");
                        bericht.Inzidenz7Tage = stateData.DataFeature[0].DataAttributes.Inz7T.ToString();
                        bericht.Stand = DateTime.Now.Date.ToString("dd.MM.yyyy");
                        string wert = GetRValue(2);
                        if (wert == null )
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

                    String urlImpfung = "https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Impfquotenmonitoring.xlsx?__blob=publicationFile";
                    var resultImpfung = GetDataSetFromLink(urlImpfung);
                    if (resultImpfung != null)
                    {
                        try
                        {
                            bericht.GesamtImpfung = Convert.ToDouble(resultImpfung.Tables[1].Rows[21][2]).ToString("#,##");
                            bericht.ErstImpfung = resultImpfung.Tables[1].Rows[21][5].ToString().Substring(0, 4).Replace(",", ".");
                            bericht.ZweitImpfung = resultImpfung.Tables[1].Rows[21][8].ToString().Substring(0, 4).Replace(",", ".");
                            bericht.ImpfStatus = true;
                        }
                        catch (Exception)
                        {
                            bericht.ImpfStatus = false;
                        }
                    }

                    bericht.StandAktuell = true;
                    return bericht;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                bericht.StandAktuell = false;
                return null;
            }
        }

        public bool SerializeRkiData()
        {
            string path = @"../SmICSWebApp/Resources/statistik/json";
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
                    status = false;
                }
            }
            else
            {
                try
                {
                    DailyReport lastReport = DeserializeRkiData(DateTime.Now);
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
                catch (Exception)
                {
                    status = false;
                }
            }

            return status;
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

        public static string SeMapColor(string inzidenz)
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
            }
            catch (Exception)
            {
                farbe = "#FFFFFF";
                return farbe;
            }
        }

        public string SetCaseColor(string tag, string vortag)
        {
            string color;
            if (tag != null && vortag != null)
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
            else
            {
                color = "#5591BB";
                return color;
            }
            
        }
    }
}
