using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten.ReceiveModel;
using SmICSCoreLib.REST;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.PatientInformation.Patient_Mibi_Labordaten
{
    public class MibiPatientLaborDataFactory : IMibiPatientLaborDataFactory
    {
        private IRestDataAccess _restData;
        List<MibiLabDataModel> mibiLabDatas;
        public MibiPatientLaborDataFactory(IRestDataAccess restData)
        {
            _restData = restData;
        }

        public List<MibiLabDataModel> Process(PatientListParameter parameter)
        {
            mibiLabDatas = new List<MibiLabDataModel>();

            List<MetaDataReceiveModel> metaDatas = _restData.AQLQuery<MetaDataReceiveModel>(AQLCatalog.CasesWithResults(parameter).Query);

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
                List<MetaDataReceiveModel> metaDatas = _restData.AQLQuery<MetaDataReceiveModel>(AQLCatalog.ReportMeta(caseID).Query);

                getSampleData(metaDatas);
            }
        }*/

        private void getCaseRequirement(List<MetaDataReceiveModel> metaDatas)
        {
            foreach (MetaDataReceiveModel metaData in metaDatas)
            {
                List<RequirementReceiveModel> requirements = _restData.AQLQuery<RequirementReceiveModel>(AQLCatalog.Requirements(metaData).Query);
            }
        }

        private void getSampleData(List<MetaDataReceiveModel> metaDatas)
        {
            foreach (MetaDataReceiveModel metaData in metaDatas)
            {
                List<SampleReceiveModel> sampleDatas = _restData.AQLQuery<SampleReceiveModel>(AQLCatalog.SamplesFromResult(metaData).Query);

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

        private void getPathogenData(List<SampleReceiveModel> sampleDatas, MetaDataReceiveModel metaData)
        {
            foreach (SampleReceiveModel sampleData in sampleDatas)
            {
                List<PathogenReceiveModel> pathogenDatas = _restData.AQLQuery<PathogenReceiveModel>(AQLCatalog.PathogensFromResult(metaData, sampleData).Query);
                
                if (pathogenDatas != null) { 
                    createMibiLabData(pathogenDatas, sampleData, metaData);
                } else {
                    System.Diagnostics.Debug.WriteLine("pathogenDatas is null");
                }
            
            }
        }

        private void createMibiLabData(List<PathogenReceiveModel> pathogenDatas, SampleReceiveModel sampleData, MetaDataReceiveModel metaData)
        {
            foreach (PathogenReceiveModel pathogenData in pathogenDatas)
            {
                MibiLabDataModel mibiLabData = new MibiLabDataModel(metaData, sampleData, pathogenData);
                mibiLabDatas.Add(mibiLabData);
            }
        }
    }
}
