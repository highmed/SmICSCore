using Newtonsoft.Json.Linq;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.Lab.EpiKurve;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmICSCoreLib.AQL.Lab
{
    public class LabData : ILabData
    {
        private IEpiCurveFactory _epiCurveFac;

        public LabData(IEpiCurveFactory epiCurveFac)
        {
            _epiCurveFac = epiCurveFac;
        }
        public List<EpiCurveModel> Labor_Epikurve(EpiCurveParameter parameter)
        {
            return _epiCurveFac.Process(parameter);
        }
    }
}
