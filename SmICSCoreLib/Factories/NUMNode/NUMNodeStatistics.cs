
using System;
using System.Collections.Generic;
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

        public static double GetMedian(List<LabPatientModel> list, int quanti, string name)
        {
            
            if(quanti != 0)
            {
                int getplace = (int)Math.Ceiling((double)(quanti / 2));
                if (quanti % 2 == 0)
                {
                    switch (name)
                    {
                        case "stay":
                            list.OrderBy(a => a.CountStays);
                            return (list.ElementAt(getplace + 1).CountStays + list.ElementAt(getplace + 2).CountStays) / 2;
                        case "nosCase":
                            list.OrderBy(a => a.CountNosCases);
                            return (list.ElementAt(getplace + 1).CountNosCases + list.ElementAt(getplace + 2).CountNosCases) / 2;
                        case "maybeNosCase":
                            list.OrderBy(a => a.CountMaybeNosCases);
                            return (list.ElementAt(getplace + 1).CountMaybeNosCases + list.ElementAt(getplace + 2).CountMaybeNosCases) / 2;
                        case "contact":
                            list.OrderBy(a => a.CountContacts);
                            return (list.ElementAt(getplace + 1).CountContacts + list.ElementAt(getplace + 2).CountContacts) / 2;
                    } 
                }else
                {
                    switch (name)
                    {
                        case "stay":
                            list.OrderBy(a => a.CountStays);
                            return list.ElementAt(getplace + 1).CountStays;
                        case "nosCase":
                            list.OrderBy(a => a.CountNosCases);
                            return list.ElementAt(getplace + 1).CountNosCases;
                        case "maybeNosCase":
                            list.OrderBy(a => a.CountMaybeNosCases);
                            return list.ElementAt(getplace + 1).CountMaybeNosCases;
                        case "contact":
                            list.OrderBy(a => a.CountContacts);
                            return list.ElementAt(getplace + 1).CountContacts;
                    }
                }
            }
            return 0;
        }

        public static double GetUnderQuartil(List<LabPatientModel> list, int quanti, string name)
        {

            if (quanti != 0)
            {
                int getplace = (int)Math.Ceiling((double)(quanti * 0.25));
                switch (name)
                {
                    case "stay":
                        list.OrderBy(a => a.CountStays);
                        return list.ElementAt(getplace + 1).CountStays;
                    case "nosCase":
                        list.OrderBy(a => a.CountNosCases);
                        return list.ElementAt(getplace + 1).CountNosCases;
                    case "maybeNosCase":
                        list.OrderBy(a => a.CountMaybeNosCases);
                        return list.ElementAt(getplace + 1).CountMaybeNosCases;
                    case "contact":
                        list.OrderBy(a => a.CountContacts);
                        return list.ElementAt(getplace + 1).CountContacts;
                }
            }
            return 0;
        }

        public static double GetUpperQuartil(List<LabPatientModel> list, int quanti, string name)
        {

            if (quanti != 0)
            {
                int getplace = (int)Math.Ceiling((double)(quanti * 0.75));
                switch (name)
                {
                    case "stay":
                        list.OrderBy(a => a.CountStays);
                        return list.ElementAt(getplace + 1).CountStays;
                    case "nosCase":
                        list.OrderBy(a => a.CountNosCases);
                        return list.ElementAt(getplace + 1).CountNosCases;
                    case "maybeNosCase":
                        list.OrderBy(a => a.CountMaybeNosCases);
                        return list.ElementAt(getplace + 1).CountMaybeNosCases;
                    case "contact":
                        list.OrderBy(a => a.CountContacts);
                        return list.ElementAt(getplace + 1).CountContacts;
                }
            }
            return 0;
        }

    }
}
