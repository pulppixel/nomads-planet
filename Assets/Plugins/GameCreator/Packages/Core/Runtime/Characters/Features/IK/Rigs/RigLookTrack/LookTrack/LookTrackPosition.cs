using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters.IK
{
    public readonly struct LookTrackPosition : ILookTrack
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] public int Layer { get; }

        public bool Exists => true;
        
        [field: NonSerialized] public Vector3 Position { get; }
        
        public GameObject Target => null;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public LookTrackPosition(int layer, Vector3 position)
        {
            this.Layer = layer;
            this.Position = position;
        }
    }
}