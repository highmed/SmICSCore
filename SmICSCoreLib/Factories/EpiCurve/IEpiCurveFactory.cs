using SmICSCoreLib.Factories.General;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.EpiCurve
{
    public interface IEpiCurveFactory
    {
        List<EpiCurveModel> Process(EpiCurveParameter parameter);
    }
}