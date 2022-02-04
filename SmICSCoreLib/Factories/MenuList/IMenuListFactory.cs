using SmICSCoreLib.REST;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.MenuList
{
    public interface IMenuListFactory
    {
        IRestDataAccess RestDataAccess { get; set; }

        List<PathogenMenuEntry> Pathogens();
        List<WardMenuEntry> Wards();
    }
}