using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Quartz;
using System;
using Microsoft.Extensions.Logging.Abstractions;
using SmICSCoreLib.DB.MenuItems;
using SmICSCoreLib.DB.Models;
using SmICSCoreLib.Factories.MenuList;
using System.Collections.Generic;
using System.Linq;

namespace SmICSCoreLib.CronJobs
{
    [DisallowConcurrentExecution]
    public class MenuItemsJob
    {
        private IMenuItemDataAccess _dataAccess;
        private IMenuListFactory _menuListFac;
        public MenuItemsJob(IMenuItemDataAccess dataAccess, IMenuListFactory menuListFac)
        {
            _dataAccess = dataAccess;
            _menuListFac = menuListFac;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Task patho = Task.Run(UpdatePathogens);
            Task wards = Task.Run(UpdateWards);
            await Task.WhenAll(patho, wards);
        }

        private async Task UpdateWards()
        {
            List<WardMenuEntry> wardMenuEntries = _menuListFac.Wards();
            List<Ward> wards = await _dataAccess.GetWards();
            foreach (WardMenuEntry ward in wardMenuEntries)
            {
                if (wards.Where(w => w.Name == w.Name).Count() == 0)
                {
                    _dataAccess.SetWard(new Ward() { Name = ward.ID });
                }
            }
            foreach (Ward ward in wards)
            {
                if (wardMenuEntries.Where(w => w.ID == ward.Name).Count() == 0)
                {
                    _dataAccess.DeleteWard(ward);
                }
            }
        }

        private async Task UpdatePathogens()
        {
            List<PathogenMenuEntry> pathoMenu = _menuListFac.Pathogens();
            List<Pathogen> pathogens = await _dataAccess.GetPathogens();
            foreach (PathogenMenuEntry pat in pathoMenu)
            {
                if (pathogens.Where(p => p.Code == pat.ID).Count() == 0)
                {
                    _dataAccess.SetPathogen(new Pathogen() { Code = pat.ID, Name = pat.Name });
                }
                else if (pathogens.Where(p => p.Code == pat.ID).Count() == 1)
                {
                    _dataAccess.UpdatePathogen(new Pathogen() { Code = pat.ID, Name = pat.Name });
                }
            }
            foreach (Pathogen pat in pathogens)
            {
                if (pathoMenu.Where(p => p.ID == pat.Code).Count() == 0)
                {
                    _dataAccess.DeletePathogen(pat);
                }
            }
        }
    }
}
