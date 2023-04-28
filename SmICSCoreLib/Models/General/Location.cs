using SmICSCoreLib.Factories.PatientMovementNew;
using System;
using System.CodeDom;

namespace SmICSCoreLib.Models.General
{
    public abstract class Location
    {
        public string? Name { get; set; }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetHashCode() == this.GetHashCode())
            {
                return true;
            }
            else if(obj is Location)
            {
                Location other = obj as Location;
                return other.Name == Name;
            }
            return false;
        }

        public static bool operator ==(Location x, Location y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Location x, Location y)
        {
            return !(x == y);
        }
    }
}
