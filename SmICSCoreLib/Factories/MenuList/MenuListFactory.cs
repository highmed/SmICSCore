using SmICSCoreLib.DB;
using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi.PatientView;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.MenuList
{
    public class MenuListFactory : IMenuListFactory
    {
        public IRestDataAccess RestDataAccess { get; set; }

        public MenuListFactory(IRestDataAccess restDataAccess)
        {
            RestDataAccess = restDataAccess;
        }

        public async Task<List<WardMenuEntry>> WardsAsync(JobType type, DateTime StartDate)
        {
            try
            {
                List<WardMenuEntry> wards = new List<WardMenuEntry>();
                DateTime end = DateTime.Now.Date;

                DateTime date = StartDate.Date;
                while (date.Date < end.Date)
                {
                    DateTime nextDate = DateTime.MinValue;
                    if (type == JobType.DAILY)
                    {
                        nextDate = date.AddDays(1.0);
                    }
                    else
                    {
                        nextDate = date.AddMonths(1);
                        if (date > nextDate)
                        {
                            nextDate = end;
                        }
                    }
                    List<UID> uids = await RestDataAccess.AQLQueryAsync<UID>(StayCompositionsPerTimespan(date, nextDate));
                    if (uids is not null)
                    {
                        int count = 0;
                        while (count < uids.Count)
                        {
                            int range = 999;
                            if ((count + range) > (uids.Count - 1))
                            {
                                range = uids.Count - count;
                            }
                            List<WardMenuEntry> tmpWards = await RestDataAccess.AQLQueryAsync<WardMenuEntry>(WardList(uids.GetRange(count, range)));
                            if (tmpWards is not null)
                            {
                                foreach (WardMenuEntry entry in tmpWards)
                                {
                                    if (!wards.Contains(entry))
                                    {
                                        wards.Add(entry);
                                    }
                                }
                            }
                            tmpWards = null;
                            count += (range + 1);
                        }
                        uids = null;
                    }
                    date = nextDate;
                }
                if (wards.Count > 0)
                {
                    return wards;
                }
                return null;
            }
            catch
            {

                throw;
            }
        }

        public async Task<List<PathogenMenuEntry>> PathogensAsync(JobType type, DateTime StartDate)
        {
            try
            {
                List<PathogenMenuEntry> pathogens = new List<PathogenMenuEntry>();
                DateTime end = DateTime.Now.Date;

                DateTime date = StartDate.Date;
                while (date.Date < end.Date)
                {
                    DateTime nextDate = DateTime.MinValue;
                    if (type == JobType.DAILY)
                    {
                        nextDate = date.AddDays(1.0);
                    }
                    else
                    {
                        nextDate = date.AddMonths(1);
                        if (date > nextDate)
                        {
                            nextDate = end;
                        }
                    }
                    List<UID> mibi_uids = await RestDataAccess.AQLQueryAsync<UID>(MibiPathogenCompositionsPerTimespan(date, nextDate));
                    await PathoEntriesAsync(mibi_uids, pathogens, MedicalField.MICROBIOLOGY);
                    mibi_uids = null;
                    List<UID> viro_uids = await RestDataAccess.AQLQueryAsync<UID>(ViroPathogenCompositionsPerTimespan(date, nextDate));
                    await PathoEntriesAsync(viro_uids, pathogens, MedicalField.VIROLOGY);
                    viro_uids = null;

                    date = nextDate;
                }

                if (pathogens.Count > 0)
                {
                    return pathogens;
                }
                return null;
            }
            catch
            {
                throw;
            }
            
        }

        private async Task PathoEntriesAsync(List<UID> uids, List<PathogenMenuEntry> pathogens, string field)
        {
            if (uids is not null)
            {
                int count = 0;
                while (count < uids.Count)
                {
                    int range = 999;
                    if ((count + range) > (uids.Count - 1))
                    {
                        range = uids.Count - count;
                    }
                    List<PathogenMenuEntry> tmpPatho = null;
                    if (field == MedicalField.MICROBIOLOGY)
                    {
                        tmpPatho = await RestDataAccess.AQLQueryAsync<PathogenMenuEntry>(MibiPathogenList(uids.GetRange(count, range)));
                    }
                    else
                    {
                        tmpPatho = await RestDataAccess.AQLQueryAsync<PathogenMenuEntry>(ViroPathogenList(uids.GetRange(count, range)));
                    }
                    if (tmpPatho is not null)
                    {
                        foreach (PathogenMenuEntry entry in tmpPatho)
                        {
                            if (!pathogens.Contains(entry))
                            {
                                pathogens.Add(entry);
                            }
                        }
                    }
                    tmpPatho = null;
                    count += (range + 1);
                }
            }
        }

        private AQLQuery StayCompositionsPerTimespan(DateTime start, DateTime end)
        {
            return new AQLQuery()
            {
                Name = "Patientaufenthalt in Zeitraum",
                Query = @$"SELECT c/uid/value as ID
                        FROM EHR e
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.event_summary.v0]
                        WHERE c/name/value='Patientenaufenthalt' 
                        AND c/context/start_time/value >= '{start.ToString("yyyy-MM-dd")}'
                        AND c/context/start_time/value < '{end.ToString("yyyy-MM-dd")}'"
            };
        }

        private AQLQuery MibiPathogenCompositionsPerTimespan(DateTime start, DateTime end)
        {
            return new AQLQuery()
            {
                Name = "Mibi Befund in Zeitraum",
                Query = @$"SELECT c/uid/value as ID
                        FROM EHR e
                        CONTAINS COMPOSITION c[openEHR-EHR-COMPOSITION.report-result.v1]
                        WHERE c/name/value='Mikrobiologischer Befund' 
                        AND c/context/start_time/value >= '{start.ToString("yyyy-MM-dd")}'
                        AND c/context/start_time/value < '{end.ToString("yyyy-MM-dd")}'"
            };
        }

        private AQLQuery ViroPathogenCompositionsPerTimespan(DateTime start, DateTime end)
        {
            return new AQLQuery()
            {
                Name = "Viro Befund in Zeitraum",
                Query = @$"SELECT c/uid/value as ID
                        FROM EHR e
                        CONTAINS COMPOSITION c
                        WHERE c/name/value='Virologischer Befund' 
                        AND c/context/start_time/value >= '{start.ToString("yyyy-MM-dd")}'
                        AND c/context/start_time/value < '{end.ToString("yyyy-MM-dd")}'"
            };
        }

        private AQLQuery WardList(List<UID> uids)
        {
            string uidList = "{'" + string.Join("','", uids.Select(i => i.ID)) + "'}";
            return new AQLQuery()
            {
                Name = "WardList",
                Query = @$"SELECT DISTINCT h/items[at0027]/value/value as ID
                        FROM EHR e
                        CONTAINS COMPOSITION c
                        CONTAINS CLUSTER h[openEHR-EHR-CLUSTER.location.v1]
                        WHERE c/uid/value MATCHES {uidList}"
            };
        }

        private AQLQuery MibiPathogenList(List<UID> uids)
        {
            string uidList = "{'" + string.Join("','", uids.Select(i => i.ID)) + "'}";
            return new AQLQuery()
            {
                Name = "PathogenList",
                Query = $@"SELECT DISTINCT w/items[at0001]/value/value as Name,
                        w/items[at0001]/value/defining_code/code_string as ID
                        FROM EHR e
                        CONTAINS COMPOSITION c
                        CONTAINS CLUSTER w[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]
                        WHERE c/name/value='Mikrobiologischer Befund' and w/items[at0001]/name/value='Erregername' AND c/uid/value MATCHES {uidList}"

            };
        } 
        private AQLQuery ViroPathogenList(List<UID> uids)
        {
            string uidList = "{'" + string.Join("','", uids.Select(i => i.ID)) + "'}";
            return new AQLQuery()
            {
                Name = "PathogenList",
                Query = $@"SELECT DISTINCT w/items[at0024]/value/value as Name,
                        w/items[at0024]/value/defining_code/code_string as ID
                        FROM EHR e
                        CONTAINS COMPOSITION c
                        CONTAINS CLUSTER w[openEHR-EHR-CLUSTER.laboratory_test_analyte.v1]
                        WHERE c/name/value='Virologischer Befund' and c/uid/value MATCHES {uidList}"

            };
        }
    }
}
