using Newtonsoft.Json.Linq;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.Lab.EpiKurve;
using SmICSCoreLib.AQL.Lab.RKIConfig;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SmICSCoreLib.REST;

namespace SmICSCoreLib.AQL.Lab
{
    public class LabData : ILabData
    {
        private IEpiCurveFactory _epiCurveFac;
        protected IRestDataAccess _restData;

        public LabData(IEpiCurveFactory epiCurveFac, IRestDataAccess restData)
        {
            _epiCurveFac = epiCurveFac;
            _restData = restData;
        }
        public List<EpiCurveModel> Labor_Epikurve(EpiCurveParameter parameter)
        {
            return _epiCurveFac.Process(parameter);
        }

        public List<LabDataKeimReceiveModel> ProcessGetErreger(string name)
        {
            List<LabDataKeimReceiveModel> erregerList = new List<LabDataKeimReceiveModel>();
            if (name == "SARS-CoV-2")
            {
                erregerList = _restData.AQLQuery<LabDataKeimReceiveModel>(AQLCatalog.GetErregernameFromViro(name));
            }
            else
            {
                erregerList = _restData.AQLQuery<LabDataKeimReceiveModel>(AQLCatalog.GetErregernameFromMikro(name));
            }

            return erregerList;
        }
    }
}
