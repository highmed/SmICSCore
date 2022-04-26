using SmICSCoreLib.DB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmICSCoreLib.DB.MenuItems
{
    public class MenuItemDataAccess : IMenuItemDataAccess
    {
        private readonly IDataAccess _data;

        public MenuItemDataAccess(IDataAccess data)
        {
            _data = data;
        }

        public async Task<List<Ward>> GetWards()
        {
            string sql = "SELECT Name FROM wards";
            List<Ward> wards = await _data.LoadData<Ward, dynamic>(sql, new { });
            return wards;
        }

        public async Task<List<Pathogen>> GetPathogens()
        {
            string sql = "SELECT Name, Code FROM pathogens";
            List<Pathogen> wards = await _data.LoadData<Pathogen, dynamic>(sql, new { });
            return wards;
        }

        public async void SetPathogen(Pathogen pathogen)
        {
            string sql = "INSERT INTO pathogens (Name, Code) VALUES(@Name, @Code)";
            await _data.SaveData(sql, new { Name = pathogen.Name, Code = pathogen.Code });
        }

        public async void SetWard(Ward ward)
        {
            string sql = "INSERT INTO wards (Name) VALUES(@Name)";
            await _data.SaveData(sql, new { Name = ward.Name });
        }

        public async void CreatePathogenTable()
        {
            string sql = "CREATE TABLE IF NOT EXISTS pathogens (Name varchar(255) NOT NULL, Code varchar(10) NOT NULL PRIMARY KEY)";
            await _data.SaveData(sql, new { });
        }

        public async void CreateWardTable()
        {
            string sql = "CREATE TABLE IF NOT EXISTS wards (Name varchar(255) NOT NULL PRIMARY KEY)";
            await _data.SaveData(sql, new { });
        }

        public async void DeleteWard(Ward ward)
        {
            string sql = "DELETE FROM wards WHERE Name=@Name";
            await _data.SaveData(sql, new { Name = ward.Name });
        }

        public async void DeletePathogen(Pathogen pathogen)
        {
            string sql = "DELETE FROM pathogens WHERE Code=@Code";
            await _data.SaveData(sql, new { Code = pathogen.Code });
        }

        public async void UpdatePathogen(Pathogen pathogen)
        {
            string sql = "UPDATE pathogens SET Name=@Name WHERE Code=@Code";
            await _data.SaveData(sql, new { Name = pathogen.Name, Code = pathogen.Code });
        }
        public async Task<List<Pathogen>> GetPathogendByName(string pathogen)
        {
            string sql = "SELECT Name, Code FROM pathogens WHERE Name=@Name OR Code=@Name";
            List<Pathogen> pathogens = await _data.LoadData<Pathogen, dynamic>(sql, new { Name=pathogen});
            return pathogens;
        }
    }
}
