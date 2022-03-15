using SmICSCoreLib.DB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmICSCoreLib.DB.MenuItems
{
    public interface IMenuItemDataAccess
    {
        void CreatePathogenTable();
        void CreateWardTable();
        Task<List<Pathogen>> GetPathogens();
        Task<List<Ward>> GetWards();
        void SetPathogen(Pathogen pathogen);
        void SetWard(Ward ward);
        void UpdatePathogen(Pathogen pathogen);
        void DeletePathogen(Pathogen pathogen);
        void DeleteWard(Ward ward);
    }
}