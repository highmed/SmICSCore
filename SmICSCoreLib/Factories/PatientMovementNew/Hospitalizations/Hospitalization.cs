﻿using SmICSCoreLib.Factories.General;
using System;
using System.Collections.Generic;

namespace SmICSCoreLib.Factories.PatientMovementNew
{
    public class Hospitalization : Case, IComparer<Hospitalization>, IComparable
    {
        public Admission Admission { get; set; }
        public Discharge Discharge { get; set; }

        public int Compare(Hospitalization x, Hospitalization y)
        {
            return x.Admission.Date.CompareTo(y.Admission.Date);
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Hospitalization other = obj as Hospitalization;
            if (other != null)
                return other.Admission.Date.CompareTo(Admission.Date);
            else
                throw new ArgumentException("Object is not a Hospitalization");
        }

        public bool Equals(Hospitalization other)
        {
            if(base.Equals(other))
            {
                if (Admission.Equals(other.Admission))
                {
                    return Discharge != null && Discharge.Equals(other.Discharge);
                }
            }
            return false;
        }
    }
}