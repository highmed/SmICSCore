using System;

namespace SmICSCoreLib.Factories.MenuList
{
    public class PathogenMenuEntry : IEquatable<PathogenMenuEntry>
    {
        public string Name { get; set; }
        public string ID { get; set; }

        public bool Equals(PathogenMenuEntry other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            if(other is not null)
            {
                if (ID == other.ID)
                {
                    return true;
                }
            }
            return false;
        }
    }
}