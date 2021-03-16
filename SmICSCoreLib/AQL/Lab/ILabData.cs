using Newtonsoft.Json.Linq;
using SmICSCoreLib.AQL.General;
using SmICSCoreLib.AQL.Lab.EpiKurve;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.Lab
{
    public interface ILabData
    {
        List<EpiCurveModel> Labor_Epikurve(EpiCurveParameter parameter);
    }
}