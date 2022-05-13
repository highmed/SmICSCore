using System;
using System.Collections.Generic;
using SmICSCoreLib.REST;
using SmICSCoreLib.Factories.General;


namespace SmICSWebApp.Data
{
    public class NUMNodeService
    {
        public int sum;
        public int average;
        public int quantity;
        public List<CSV> nodeNodeList;
        private readonly string path = @"./Resources/";
        public int GetAverage(int parameter, int quanti)
        {   
            if(parameter != 0 & quanti != 0)
            {
                average = parameter / quanti;
                return average;
            }
            else
            {
                return average = 0;
            }
        }

        public void GetSum(int parameter, string source)
        {
            sum = 0;
            for(int i = 0; i < parameter; i++)
            {
                sum =+ parameter;
                quantity = i;
            }
            average = GetAverage(sum, quantity);
            if(average != 0)
            {
                DateTime date = DateTime.Now;
                CreateCSVObject(average, source, date);
            } 
        }

        public void CreateCSVObject(int parameter, string name, DateTime date)
        {
            switch (name)
            {
                case "stays": nodeNodeList.Add(new CSV { AverageNumberOfStays = parameter, Date = date }); break;
                case "cases": nodeNodeList.Add(new CSV { AverageNumberOfNosCases = parameter, Date = date }); break;
                case "contacts": nodeNodeList.Add(new CSV { AverageNumberOfContacts = parameter, Date = date }); break;
            }
            SaveCSV.ToCsv(nodeNodeList, path);
            
        }
    }
}
