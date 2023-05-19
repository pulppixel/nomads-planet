using System;
using Unity.Mathematics;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public readonly struct ClusterKey : IEquatable<ClusterKey>
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private readonly int size;
        [NonSerialized] private readonly Vector3Int hash;

        [NonSerialized] private readonly int m_HashCode;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ClusterKey(int size, Vector3Int hash)
        {
            this.size = size;
            this.hash = hash;
            
            this.m_HashCode = HashCode.Combine(size, hash);
        }
        
        // OPERATORS: -----------------------------------------------------------------------------

        public static bool operator ==(ClusterKey left, ClusterKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ClusterKey left, ClusterKey right)
        {
            return !left.Equals(right);
        }
        
        // EQUALITY: ------------------------------------------------------------------------------
        
        public bool Equals(ClusterKey other)
        {
            return hash.x == other.hash.x &&
                   hash.y == other.hash.y &&
                   hash.z == other.hash.z &&
                   size == other.size;
        }

        public override bool Equals(object other)
        {
            return other is ClusterKey otherClusterEntry && Equals(otherClusterEntry);
        }

        public override int GetHashCode()
        {
            return this.m_HashCode;
        }
        
        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static Vector3Int Hash(int clusterSize, float3 position)
        {
            return new Vector3Int(
                (int) math.floor(position.x / clusterSize),
                (int) math.floor(position.y / clusterSize),
                (int) math.floor(position.z / clusterSize)
            );
        }
    }
}