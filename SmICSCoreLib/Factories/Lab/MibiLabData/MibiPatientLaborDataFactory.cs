using SmICSCoreLib.Factories.General;
using SmICSCoreLib.Factories.MiBi;
using SmICSCoreLib.REST;
using System.Collections.Generic;
using SmICSCoreLib.Factories.Lab.MibiLabdata.ReceiveModel;
using System.Threading.Tasks;

namespace SmICSCoreLib.Factories.Lab.MibiLabData
{
    public class MibiPatientLaborDataFactory : IMibiPatientLaborDataFactory
    {
        private IRestDataAccess _restData;
        private IAntibiogramFactory _antibiogram;
        List<MibiLabDataModel> mibiLabDatas;
        public MibiPatientLaborDataFactory(IRestDataAccess restData, IAntibiogramFactory antibiogram)
        {
            _restData = restData;
            _antibiogram = antibiogram;
        }

        public List<MibiLabDataModel> Process(PatientListParameter parameter)
        {
            mibiLabDatas = new List<MibiLabDataModel>();

            List<MetaDataReceiveModel> metaDatas = _restData.AQLQueryAsync<MetaDataReceiveModel>(AQLCatalog.CasesWithResults(parameter)).GetAwaiter().GetResult();

            if (metaDatas != null)
            {
                getSampleData(metaDatas);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("metaDatas is null");
            }

            return mibiLabDatas;
        }

        /*private void getAllUIDs(List<CaseIDReceiveModel> cases)
        {
            foreach (CaseIDReceiveModel caseID in cases)
            {
                List<MetaDataReceiveModel> metaDatas = _restData.AQLQueryAsync<MetaDataReceiveModel>(AQLCatalog.ReportMeta(caseID);

                getSampleData(metaDatas);
            }
        }*/

        private void getCaseRequirement(List<MetaDataReceiveModel> metaDatas)
        {
            foreach (MetaDataReceiveModel metaData in metaDatas)
            {
                List<RequirementReceiveModel> requirements = _restData.AQLQueryAsync<RequirementReceiveModel>(AQLCatalog.Requirements(metaData)).GetAwaiter().GetResult();
            }
        }

        private void getSampleData(List<MetaDataReceiveModel> metaDatas)
        {
            foreach (MetaDataReceiveModel metaData in metaDatas)
            {
                List<SampleReceiveModel> sampleDatas = _restData.AQLQueryAsync<SampleReceiveModel>(AQLCatalog.SamplesFromResult(metaData)).GetAwaiter().GetResult();

                if (sampleDatas != null)
                {
                    getPathogenData(sampleDatas, metaData);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("sampleDatas is null");
                }
            }
        }

        private async Task getPathogenData(List<SampleReceiveModel> sampleDatas, MetaDataReceiveModel metaData)
        {
            foreach (SampleReceiveModel sampleData in sampleDatas)
            {
                List<PathogenReceiveModel> pathogenDatas = await _restData.AQLQueryAsync<PathogenReceiveModel>(AQLCatalog.PathogensFromResult(metaData, sampleData));

                if (pathogenDatas != null)
                {
                    createMibiLabData(pathogenDatas, sampleData, metaData);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("pathogenDatas is null");
                }

            }
        }

        private async Task createMibiLabData(List<PathogenReceiveModel> pathogenDatas, SampleReceiveModel sampleData, MetaDataReceiveModel metaData)
        {
            foreach (PathogenReceiveModel pathogenData in pathogenDatas)
            {
                MibiLabDataModel mibiLabData = new MibiLabDataModel(metaData, sampleData, pathogenData);
                AntibiogramParameter antibiogramParameter = new AntibiogramParameter()
                {
                    Pathogen = pathogenData.KeimID,
                    LabID = sampleData.LabordatenID,
                    UID = metaData.UID,
                    IsolatNo = pathogenData.IsolatNo
                };
                mibiLabData.Antibiogram = await _antibiogram.ProcessAsync(antibiogramParameter);
                mibiLabDatas.Add(mibiLabData);
            }
        }
    }
}
