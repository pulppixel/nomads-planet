using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public interface ISpatialHash
    {
        Vector3 Position { get; }
        int UniqueCode { get; }
    }
}