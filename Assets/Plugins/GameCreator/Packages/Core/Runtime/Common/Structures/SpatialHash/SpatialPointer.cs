using System;

namespace GameCreator.Runtime.Common
{
    public readonly struct SpatialPointer : IComparable<SpatialPointer>, IEquatable<SpatialPointer>
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] public int UniqueCode { get; }
        [field: NonSerialized] public float Distance { get; }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public SpatialPointer(int uniqueCode, float distance)
        {
            this.UniqueCode = uniqueCode;
            this.Distance = distance;
        }
        
        // METHODS: -------------------------------------------------------------------------------
        
        public int CompareTo(SpatialPointer other)
        {
            return this.Distance.CompareTo(other.Distance);
        }

        public bool Equals(SpatialPointer other)
        {
            return this.UniqueCode == other.UniqueCode;
        }

        public override bool Equals(object other)
        {
            return other is SpatialPointer otherSpatialPointer && this.Equals(otherSpatialPointer);
        }

        public override int GetHashCode()
        {
            return this.UniqueCode;
        }

        public static bool operator ==(SpatialPointer left, SpatialPointer right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SpatialPointer left, SpatialPointer right)
        {
            return !left.Equals(right);
        }
    }
}