using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    internal class BoneSnapshot
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public Transform Value { get; }

        public Vector3 LocalPosition { get; private set; }
        public Vector3 WorldPosition { get; private set; }

        public Quaternion LocalRotation { get; private set; }
        public Quaternion WorldRotation { get; private set; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public BoneSnapshot(Transform reference)
        {
            this.Value = reference;
            this.Snapshot();
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Snapshot()
        {
            this.WorldPosition = this.Value.position;
            this.LocalPosition = this.Value.localPosition;

            this.WorldRotation = this.Value.rotation;
            this.LocalRotation = this.Value.localRotation;
        }
    }
}