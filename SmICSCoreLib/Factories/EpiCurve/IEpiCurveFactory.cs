using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.EpiCurve
{
    public interface IEpiCurveFactory
    {
        IRestDataAccess RestDataAccess { get; }
        List<EpiCurveModel> Process(EpiCurveParameter parameter);
    }
}