
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

        public static (double, double, double) GetMedianAndInterquartil(List<LabPatientModel> list, int quanti, string name)
        {
            
            if(quanti != 0)
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
            }
            return (0, 0, 0);
        }

    }
}
