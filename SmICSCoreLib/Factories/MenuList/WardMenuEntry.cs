using System;

namespace SmICSCoreLib.Factories.MenuList
{
    public class WardMenuEntry : IEquatable<WardMenuEntry>
    {
        public string ID { get; set; }

        public bool Equals(WardMenuEntry other)
        {
            if(ReferenceEquals(this,other))
            {
                return true;
            }
            if(other is not null)
            {
                if(ID == other.ID)
                {
                    return true;
                }
            }
            return false;
        }
    }
}