using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    internal class SpatialItem
    {
        [field: NonSerialized] public ISpatialHash Instance { get; }
        [field: NonSerialized] public Vector3 Position { get; set; }

        public int UniqueCode => this.Instance.UniqueCode;

        public SpatialItem(ISpatialHash instance)
        {
            this.Instance = instance;
            this.Position = instance.Position;
        }
    }
}