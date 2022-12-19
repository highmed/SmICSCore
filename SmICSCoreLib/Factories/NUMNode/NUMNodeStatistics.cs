
using SmICSCoreLib.JSONFileStream;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmICSCoreLib.Factories.NUMNode
{
    public static class NUMNodeStatistics
    {

        public static double GetAverage(int parameter, int quanti)
        {
            if (quanti != 0)
            {
                double average = (double)parameter / (double)quanti;
                return average;
            }
            else
            {
                return 0;
            }
        }

        public static (double, double, double) GetMedianAndInterquartil(List<LabPatientModel> list, int quanti, string name)
        {
            
            if(quanti != 0 && list is not null && list.Count != 0)
            {
                int getplace_median = (int)Math.Ceiling((double)(quanti / 2));
                int getplace_underquartil = (int)Math.Ceiling((double)(quanti * 0.25));
                int getplace_upperquartil = (int)Math.Ceiling((double)(quanti * 0.75));

                double median;
                double underquartil;
                double upperquartil;

                if (quanti % 2 == 0)
                {
                    switch (name)
                    {
                        case "stay":
                            median = (list.OrderBy(a => a.CountStays).ElementAt(getplace_median - 1).CountStays + list.OrderBy(a => a.CountStays).ElementAt(getplace_median).CountStays) / 2;
                            underquartil = list.OrderBy(a => a.CountStays).ElementAt(getplace_underquartil - 1).CountStays;
                            upperquartil = list.OrderBy(a => a.CountStays).ElementAt(getplace_upperquartil - 1).CountStays;
                            return (median, underquartil, upperquartil);
                        case "nosCase":
                            median = (list.OrderBy(a => a.CountNosCases).ElementAt(getplace_median - 1).CountNosCases + list.OrderBy(a => a.CountNosCases).ElementAt(getplace_median).CountNosCases) / 2;
                            underquartil = list.OrderBy(a => a.CountNosCases).ElementAt(getplace_underquartil - 1).CountNosCases;
                            upperquartil = list.OrderBy(a => a.CountNosCases).ElementAt(getplace_upperquartil - 1).CountNosCases;
                            return (median, underquartil, upperquartil);
                        case "maybeNosCase":
                            median = (list.OrderBy(a => a.CountMaybeNosCases).ElementAt(getplace_median - 1).CountMaybeNosCases + list.OrderBy(a => a.CountMaybeNosCases).ElementAt(getplace_median).CountMaybeNosCases) / 2;
                            underquartil = list.OrderBy(a => a.CountMaybeNosCases).ElementAt(getplace_underquartil - 1).CountMaybeNosCases;
                            upperquartil = list.OrderBy(a => a.CountMaybeNosCases).ElementAt(getplace_upperquartil - 1).CountMaybeNosCases;
                            return (median, underquartil, upperquartil);
                        case "contact":
                            median = (list.OrderBy(a => a.CountContacts).ElementAt(getplace_median - 1).CountContacts + list.OrderBy(a => a.CountContacts).ElementAt(getplace_median).CountContacts) / 2;
                            underquartil = list.OrderBy(a => a.CountContacts).ElementAt(getplace_underquartil - 1).CountContacts;
                            upperquartil = list.OrderBy(a => a.CountContacts).ElementAt(getplace_upperquartil - 1).CountContacts;
                            return (median, underquartil, upperquartil);
                    }
                }
                else
                {
                    switch (name)
                    {
                        case "stay":
                            median = list.OrderBy(a => a.CountStays).ElementAt(getplace_median - 1).CountStays;
                            underquartil = list.OrderBy(a => a.CountStays).ElementAt(getplace_underquartil - 1).CountStays;
                            upperquartil = list.OrderBy(a => a.CountStays).ElementAt(getplace_upperquartil - 1).CountStays;
                            return (median, underquartil, upperquartil);
                        case "nosCase":
                            median = list.OrderBy(a => a.CountNosCases).ElementAt(getplace_median - 1).CountNosCases;
                            underquartil = list.OrderBy(a => a.CountNosCases).ElementAt(getplace_underquartil - 1).CountNosCases;
                            upperquartil = list.OrderBy(a => a.CountNosCases).ElementAt(getplace_upperquartil - 1).CountNosCases;
                            return (median, underquartil, upperquartil);
                        case "maybeNosCase":
                            median = list.OrderBy(a => a.CountMaybeNosCases).ElementAt(getplace_median - 1).CountMaybeNosCases;
                            underquartil = list.OrderBy(a => a.CountMaybeNosCases).ElementAt(getplace_underquartil - 1).CountMaybeNosCases;
                            upperquartil = list.OrderBy(a => a.CountMaybeNosCases).ElementAt(getplace_upperquartil - 1).CountMaybeNosCases;
                            return (median, underquartil, upperquartil);
                        case "contact":
                            median = list.OrderBy(a => a.CountContacts).ElementAt(getplace_median - 1).CountContacts;
                            underquartil = list.OrderBy(a => a.CountContacts).ElementAt(getplace_underquartil - 1).CountContacts;
                            upperquartil = list.OrderBy(a => a.CountContacts).ElementAt(getplace_upperquartil - 1).CountContacts;
                            return (median, underquartil, upperquartil);
                    }
                }
            }else if(list.Count == 0)
            {
                string itemname = "";
                switch (name)
                {
                    case "stay": itemname = "current.patientstays"; break;
                    case "nosCase": itemname = "current.patientnoscases"; break;
                    case "maybeNosCase": itemname = "current.patientmaybenoscases"; break;
                    case "contact": itemname = "current.patientcontacts"; break;
                }

                string path = @"../SmICSWebApp/Resources/NUMNode/";
                return GetOldFile(path, itemname);

            }
            return (0, 0, 0);
        }

        public static double GetStandardDeviation(List<LabPatientModel> list, int quanti, double average, string name)
        {
            if(quanti != 0)
            {
                double cal = 0;
                
                foreach(LabPatientModel item in list)
                {
                    switch (name)
                    {
                        case "stay": cal += Math.Pow(item.CountStays - average, 2); ; break;
                        case "nosCase": cal += Math.Pow(item.CountNosCases - average, 2); ; break;
                        case "maybeNosCase": cal += Math.Pow(item.CountMaybeNosCases - average, 2); break;
                        case "contact": cal += Math.Pow(item.CountContacts - average, 2); break;
                    }
                }

                return Math.Round(Math.Sqrt(cal / (quanti - 1)), 2);
            }
            return 0;
        }

        public static (double, double, double) GetOldFile(string path, string dataitem)
        {
            var directory = new DirectoryInfo(path);
            var file = (from f in directory.GetFiles()
                        orderby f.LastWriteTime descending
                        select f).FirstOrDefault();
            string getDate = file.Name.Substring(10);
            string filepath;
            NUMNodeModel report;
            double item_1 = 0, item_2 = 0, item_3 = 0;

            filepath = path + "NUMNode_R" + getDate;
            report = JSONReader<NUMNodeModel>.ReadObject(filepath);
            foreach (var item in report.dataitems)
            {
                if(item.itemname == dataitem)
                {
                    (item_1, item_2, item_3) = (item.data.median, item.data.underquartil, item.data.upperquartil);
                }
            }
                    
            return (item_1, item_2, item_3);
        }

    }
}
