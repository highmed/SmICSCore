using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.RKIStatistics.Models
{
    public class RKIDailyReportModel
    {
        [JsonProperty(PropertyName = "Timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty(PropertyName = "CurrentStatus")]
        public bool CurrentStatus { get; set; }

        [JsonProperty(PropertyName = "CaseNumbers")]
        public int CaseNumbers { get; set; }

        [JsonProperty(PropertyName = "PreCaseNumbers")]
        public int PreCaseNumbers { get; set; }

        [JsonProperty(PropertyName = "DeathCases")]
        public int DeathCases { get; set; }

        [JsonProperty(PropertyName = "PreDeathCases")]
        public int PreDeathCases { get; set; }

        [JsonProperty(PropertyName = "RValue7Days")]
        public double RValue7Days { get; set; }

        [JsonProperty(PropertyName = "RValue7PreDays")]
        public double RValue7PreDays { get; set; }

        [JsonProperty(PropertyName = "Inzidenz7Days")]
        public double Inzidenz7Days { get; set; }

        [JsonProperty(PropertyName = "Inzidenz7PreDays")]
        public double Inzidenz7PreDays { get; set; }

        [JsonProperty(PropertyName = "AllVaccinations")]
        public double AllVaccinations { get; set; }

        [JsonProperty(PropertyName = "VaccinStatus")]
        public bool VaccinStatus { get; set; }

        [JsonProperty(PropertyName = "FirstVaccination")]
        public double FirstVaccination { get; set; }

        [JsonProperty(PropertyName = "SecondVaccination")]
        public double SecondVaccination { get; set; }

        [JsonProperty(PropertyName = "FirstBooster")]
        public double FirstBooster { get; set; }

        [JsonProperty(PropertyName = "SecondBooster")]
        public double SecondBooster { get; set; }

        [JsonProperty(PropertyName = "State")]
        public List<RKIDailyReporStateModel> State { get; set; }

        [JsonProperty(PropertyName = "BLCurrentStatus")]
        public bool BLCurrentStatus { get; set; }

    }
}