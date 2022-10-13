using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using SmICSCoreLib.Factories.RKIStatistics;
using SmICSCoreLib.Factories.RKIStatistics.Models;
using SmICSCoreLib.Factories.RKIStatistics.ReceiveModels;
using SmICSCoreLib.JSONFileStream;

namespace SmICSCoreLib.CronJobs
{
    [DisallowConcurrentExecution]
    public class RKIStatisticsJob : IJob
    {
        private readonly IRKIReportFactory _listFac;
        private readonly ILogger<RKIStatisticsJob> _logger;
        private readonly string path = @"../SmICSWebApp/Resources/RKIReportData/DailyReport";
        private readonly string shortPath = @"../SmICSWebApp/Resources/RKIReportData/";

        public RKIDailyReportModel dailyReport;
        public RKIDailyReporStateModel dailyReportState;
        public List<RKIDailyReporStateModel> dailyReportStateList;
        public RKIDailyReporDistrictModel dailyReportDistrict;
        public List<RKIDailyReporDistrictModel> dailyReportDistrictList;
        public string[] states;

        private readonly string url = "https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Fallzahlen_Kum_Tab_aktuell.xlsx?__blob=publicationFile";
        private readonly string urlImpfung = "https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Impfquotenmonitoring.xlsx?__blob=publicationFile";
        private readonly string archivUrl = "https://www.rki.de/DE/Content/InfAZ/N/Neuartiges_Coronavirus/Daten/Fallzahlen_Kum_Tab_Archiv.xlsx?__blob=publicationFile";

        private Task<RKIReportFeatures<RKIReportStateCaseModel>> stateCaseData;
        private Task<List<RKIReportFeatures<RKIReportStateModel>>> stateData;
        private Task<List<RKIReportFeatures<RKIReportDistrictModel>>> districtData;

        private List<List<RKIReportFeatures<RKIReportDistrictModel>>> LK_lists;

        public RKIStatisticsJob(IRKIReportFactory listFac, ILogger<RKIStatisticsJob> logger)
        {
            _listFac = listFac;
            _logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            if (!Directory.Exists(shortPath))
            {
                Directory.CreateDirectory(shortPath);
            }
            if (IsDirectoryEmpty(shortPath))
            {
                _ = CreateReport(archivUrl);
                
            }
            _ = CreateReport(url);

            try
            {
                _ = FillDailyReport();
                _logger.LogInformation("RKIDailyReport could succefully been created.");

            }catch
            {
                _logger.LogError("RKIDailyReport could not been created.");
            }
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                //need to get old data
            }
            string filename = "RKI_DailyReport_" + DateTime.Now.ToString("yyyy-MM-dd");

            JSONWriter.Write(dailyReport, path, filename);

            return Task.CompletedTask;
        }
        private static bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        private void InitializeGlobalVariables()
        {
            dailyReport = new RKIDailyReportModel();
            dailyReportState = new RKIDailyReporStateModel();
            dailyReportDistrict = new RKIDailyReporDistrictModel();
            states = new string[] { "Baden-Württemberg", "Bayern", "Berlin","Brandenburg", "Bremen", "Hamburg",
                        "Hessen", "Mecklenburg-Vorpommern", "Niedersachsen", "Nordrhein-Westfalen", "Rheinland-Pfalz", "Saarland",
                        "Sachsen", "Sachsen-Anhalt", "Schleswig-Holstein", "Thüringen"};
        }

        public async Task FillDailyReport()
        {
            InitializeGlobalVariables();
            try
            {
                List<string> list = new();
                list = GetLKNames();
                dailyReport.Timestamp = DateTime.Now;
                dailyReport.CurrentStatus = true;

                var tasks = new Task[]
                {
                    stateCaseData = _listFac.GetStateData(0),
                    _listFac.GetAllStates(),
                    stateData = _listFac.GetStateByName(),
                    districtData = _listFac.GetDistrictByName(list)
                };

                foreach (var task in tasks)
                {
                    await task;
                }

                if (stateCaseData is not null)
                {
                    dailyReport.CaseNumbers = stateCaseData.Result.features[0].attributes.AnzFallNeu;
                    dailyReport.PreCaseNumbers = stateCaseData.Result.features[0].attributes.AnzFall;
                    dailyReport.DeathCases = stateCaseData.Result.features[0].attributes.AnzTodesfallNeu;
                    dailyReport.PreDeathCases = stateCaseData.Result.features[0].attributes.AnzTodesfall;
                    dailyReport.Inzidenz7Days = stateCaseData.Result.features[0].attributes.Inz7T;

                    dailyReport.RValue7Days = _listFac.GetRValue(2);
                    dailyReport.RValue7PreDays = _listFac.GetRValue(3);
                }

                try
                {
                    DataTable resultImpfung = DownloadFile.DownloadFile.GetInstance(urlImpfung).Sheets[1];

                    if (resultImpfung != null)
                    {
                        dailyReport.AllVaccinations = Convert.ToDouble(resultImpfung.Rows[20][2]);
                        dailyReport.FirstVaccination = Convert.ToDouble(resultImpfung.Rows[20][3]);
                        dailyReport.SecondVaccination = Convert.ToDouble(resultImpfung.Rows[20][4]);
                        dailyReport.FirstBooster = Convert.ToDouble(resultImpfung.Rows[20][5]);
                        dailyReport.SecondBooster = Convert.ToDouble(resultImpfung.Rows[20][6]);
                        dailyReport.VaccinStatus = true;
                    }
                    _logger.LogInformation("RKI vaccination data could been aquired");
                }
                catch
                {
                    _logger.LogError("RKI vaccination data could not been aquired");
                }
                

                var stateDataList = await stateData;
                await SortLKsAsync();

                if (stateDataList != null)
                {
                    dailyReportStateList = new List<RKIDailyReporStateModel>();
                    int i = 0;
                    foreach (var state in stateDataList)
                    {
                        dailyReportState = new RKIDailyReporStateModel
                        {
                            Name = state.features[0].attributes.Bundesland,

                            CaseNumbers = state.features[0].attributes.Fallzahl,
                            Cases7BL = state.features[0].attributes.Cases7_bl,
                            CasesPer100000Citizens = state.features[0].attributes.FaellePro100000Ew,
                            Deathcases = state.features[0].attributes.Todesfaelle,
                            Deathcases7BL = state.features[0].attributes.Death7_bl,
                            Inzidenz7Days = state.features[0].attributes.Faelle7BlPro100K
                        };
                        dailyReportState.Color = SetMapColor(dailyReportState.Inzidenz7Days);

                        var getRightList = LK_lists[i];
                        dailyReportDistrictList = new List<RKIDailyReporDistrictModel>();

                        foreach (var itemList in getRightList)
                        {
                            dailyReportDistrict = new RKIDailyReporDistrictModel
                            {
                                City = itemList.features[0].attributes.County,
                                DistrictName = itemList.features[0].attributes.GEN,
                                CaseNumbers = itemList.features[0].attributes.Cases,
                                Cases7LK = itemList.features[0].attributes.Cases7_lk,
                                CasesPer100000Citizens = itemList.features[0].attributes.Cases_per_100k,
                                Inzidenz7Days = itemList.features[0].attributes.Cases7_per_100k,
                                Deathcases = itemList.features[0].attributes.Deaths,
                                Deathcases7LK = itemList.features[0].attributes.Death7_lk,
                                AdmUnitID = itemList.features[0].attributes.AdmUnitId
                            };

                            dailyReportDistrictList.Add(dailyReportDistrict);
                        }
                        dailyReportState.District = dailyReportDistrictList;
                        dailyReportStateList.Add(dailyReportState);

                        i++;
                    }
                    dailyReport.State = dailyReportStateList;
                }

                dailyReport.BLCurrentStatus = true;

            }
            catch
            {
                _logger.LogError("DailyReport could not be proceed");
            }
        }

        public string SetMapColor(double inzidenz)
        {
            string farbe;
            try
            {
                int zahl = (int)Convert.ToInt64(Math.Floor(inzidenz));

                if (zahl >= 600)
                {
                    farbe = "#483259";
                }else if (zahl < 600 && zahl >= 500)
                {
                    farbe = "#8f63b0";
                }else if (zahl < 500 && zahl >= 400)
                {
                    farbe = "#c161a7";
                }else if (zahl < 400 && zahl >= 300)
                {
                    farbe = "#e56379";
                }else if (zahl < 300 && zahl >= 200)
                {
                    farbe = "#fb8f5d";
                }else if (zahl < 200 && zahl > 100)
                {
                    farbe = "#f9ba78";
                }else if (zahl < 100)
                {
                    farbe = "#ffe7b9";
                }else
                {
                    farbe = "#FFFFFF";
                }
                return farbe;
            }
            catch (Exception e)
            {
                _logger.LogWarning("SetMapColor " + e.Message);
                farbe = "#FFFFFF";
                return farbe;
            }
        }

        private List<string> GetLKNames()
        {
            DataTable result = DownloadFile.DownloadFile.GetInstance(url).Sheets[4];
            if (result != null)
            {
                var endResult = result.AsEnumerable().Skip(5).Take(418).CopyToDataTable();

                List<string> list = new(endResult.Rows.Count);
                foreach (DataRow row in endResult.Rows)
                {
                    list.Add(row[0].ToString().Replace("LK", "").Replace("SK", "").Trim());
                }                   
                return list;
            }
            return null;
        }

        private async Task SortLKsAsync()
        {
            var districtList = await districtData;
            List<RKIReportFeatures<RKIReportDistrictModel>> list_BL_1 = new();
            List<RKIReportFeatures<RKIReportDistrictModel>> list_BL_2 = new();
            List<RKIReportFeatures<RKIReportDistrictModel>> list_BL_3 = new();
            List<RKIReportFeatures<RKIReportDistrictModel>> list_BL_4 = new();
            List<RKIReportFeatures<RKIReportDistrictModel>> list_BL_5 = new();
            List<RKIReportFeatures<RKIReportDistrictModel>> list_BL_6 = new();
            List<RKIReportFeatures<RKIReportDistrictModel>> list_BL_7 = new();
            List<RKIReportFeatures<RKIReportDistrictModel>> list_BL_8 = new();
            List<RKIReportFeatures<RKIReportDistrictModel>> list_BL_9 = new();
            List<RKIReportFeatures<RKIReportDistrictModel>> list_BL_10 = new();
            List<RKIReportFeatures<RKIReportDistrictModel>> list_BL_11 = new();
            List<RKIReportFeatures<RKIReportDistrictModel>> list_BL_12 = new();
            List<RKIReportFeatures<RKIReportDistrictModel>> list_BL_13 = new();
            List<RKIReportFeatures<RKIReportDistrictModel>> list_BL_14 = new();
            List<RKIReportFeatures<RKIReportDistrictModel>> list_BL_15 = new();
            List<RKIReportFeatures<RKIReportDistrictModel>> list_BL_16 = new();

            foreach (var district in districtList)
            {

                switch (district.features[0].attributes.BL)
                {
                    case "Baden-Württemberg":
                        list_BL_1.Add(district); break;
                    case "Bayern":
                        list_BL_2.Add(district); break;
                    case "Berlin":
                        list_BL_3.Add(district); break;
                    case "Brandenburg":
                        list_BL_4.Add(district); break;
                    case "Bremen":
                        list_BL_5.Add(district); break;
                    case "Hamburg":
                        list_BL_6.Add(district); break;
                    case "Hessen":
                        list_BL_7.Add(district); break;
                    case "Mecklenburg-Vorpommern":
                        list_BL_8.Add(district); break;
                    case "Niedersachsen":
                        list_BL_9.Add(district); break;
                    case "Nordrhein-Westfalen":
                        list_BL_10.Add(district); break;
                    case "Rheinland-Pfalz":
                        list_BL_11.Add(district); break;
                    case "Saarland":
                        list_BL_12.Add(district); break;
                    case "Sachsen":
                        list_BL_13.Add(district); break;
                    case "Sachsen-Anhalt":
                        list_BL_14.Add(district); break;
                    case "Schleswig-Holstein":
                        list_BL_15.Add(district); break;
                    case "Thüringen":
                        list_BL_16.Add(district); break;
                    case "": break;
                    case null: break;
                }
            }
            LK_lists = new List<List<RKIReportFeatures<RKIReportDistrictModel>>>
            {
                list_BL_1,
                list_BL_2,
                list_BL_3,
                list_BL_4,
                list_BL_5,
                list_BL_6,
                list_BL_7,
                list_BL_8,
                list_BL_9,
                list_BL_10,
                list_BL_11,
                list_BL_12,
                list_BL_13,
                list_BL_14,
                list_BL_15,
                list_BL_16
            };
        }

        private async Task CreateReport(string link)
        {
            DataTable resultSetBL;
            DataTable resultSetLK;
            DataTable resultHeader;

            if (link == archivUrl)
            {
                resultSetBL = DownloadFile.DownloadFile.GetInstance(link).Sheets[1];
                resultSetLK = DownloadFile.DownloadFile.GetInstance(link).Sheets[3];
            }else
            {
                resultSetLK = DownloadFile.DownloadFile.GetInstance(link).Sheets[4];
                resultSetBL = DownloadFile.DownloadFile.GetInstance(link).Sheets[2];
            }
            var endresultSetBL = resultSetBL.AsEnumerable().Skip(5).CopyToDataTable();
            var endresultSetLK = resultSetBL.AsEnumerable().Skip(5).CopyToDataTable();
            resultHeader = resultSetBL;
            resultHeader.Columns.RemoveAt(0);
            var resultheader = resultHeader.AsEnumerable().ElementAt(4);
            
            RKIReportModel model = new()
            {
                Timestamp = DateTime.Now
            };

            if (resultSetBL != null)
            {
                string filename = "RKI_BLReport";
                string filePath = shortPath + "/" + filename + ".json";
                var result = endresultSetBL.Rows;
                RKIReportModel oldModel = new();

                if (File.Exists(filePath))
                {
                    oldModel = JSONReader<RKIReportModel>.ReadObject(filePath);
                }

                List<RKIReportLocationModel> rKIReportLocationModels = FillLocationModel(result, oldModel, "Bundesland", filePath, resultheader);
                model.Data = rKIReportLocationModels;

                JSONWriter.Write(model, shortPath, filename);
            }

            if (resultSetLK != null)
            {
                string filename = "RKI_LKReport";
                string filePath = shortPath + "/" + filename + ".json";

                if (link == archivUrl)
                {
                    resultSetLK.Columns.RemoveAt(0);
                }

                var result = endresultSetLK.Rows;
                RKIReportModel oldModel = new();

                if (File.Exists(filePath))
                {
                    oldModel = JSONReader<RKIReportModel>.ReadObject(filePath);
                }

                List<RKIReportLocationModel> rKIReportLocationModels = FillLocationModel(result, oldModel, "Landkreis", filePath, resultheader);
                model.Data = rKIReportLocationModels;

                JSONWriter.Write(model, shortPath, filename);
            }

            await Task.CompletedTask;
        }

        private static List<RKIReportLocationModel> FillLocationModel(DataRowCollection dataRowCollection, RKIReportModel rKIReportModel, string description, string file, DataRow header)
        {
            List<RKIReportLocationModel> rKIReportLocationModels = new();
            RKIReportLocationModel rKIReportLocationModel = new();
            List<RKIReportCaseModel> rKIReportCaseModels = new();

            dataRowCollection.RemoveAt(dataRowCollection.Count);

            foreach (DataRow row in dataRowCollection)
            {
                rKIReportLocationModel.Description = description;
                rKIReportLocationModel.Name = row[0].ToString();
                if(description == "Landkreis")
                {
                    rKIReportLocationModel.ID = row[1].ToString();
                }
                
                if (File.Exists(file))
                {
                    foreach (RKIReportLocationModel item in rKIReportModel.Data.Where(x => x.Name == row[0].ToString()))
                    {
                        foreach (RKIReportCaseModel caseModel in item.CaseNumbers)
                        {
                            RKIReportCaseModel rKIReportCaseModel = new()
                            {
                                Timestamp = caseModel.Timestamp,
                                CaseNumber = caseModel.CaseNumber
                            };
                            rKIReportCaseModels.Add(rKIReportCaseModel);
                        }
                    }
                }

                for (int i = 1; i < row.Table.Columns.Count; i++)
                {
                    RKIReportCaseModel rKIReportCaseModel = new()
                    {
                        Timestamp = Convert.ToDateTime(header[i-1]),
                        CaseNumber = Convert.ToInt32(row[i])
                    };
                    rKIReportCaseModels.Add(rKIReportCaseModel);
                }
                rKIReportLocationModel.CaseNumbers = rKIReportCaseModels;
                rKIReportLocationModels.Add(rKIReportLocationModel);
            }
            return rKIReportLocationModels;
        }
    }
}
