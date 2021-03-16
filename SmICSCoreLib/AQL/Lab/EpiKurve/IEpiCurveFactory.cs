using SmICSCoreLib.AQL.General;
using System.Collections.Generic;

namespace SmICSCoreLib.AQL.Lab.EpiKurve
{
    public interface IEpiCurveFactory
    {
        List<EpiCurveModel> Process(EpiCurveParameter parameter);
    }
}