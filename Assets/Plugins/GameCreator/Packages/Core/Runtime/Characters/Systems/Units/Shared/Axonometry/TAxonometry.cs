using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public abstract class TAxonometry : IAxonometry
    {
        public virtual Vector3 ProcessTranslation(TUnitDriver driver, Vector3 movement)
        {
            return movement;
        }

        public virtual void ProcessPosition(TUnitDriver driver, Vector3 position)
        { }

        public virtual Vector3 ProcessRotation(TUnitFacing facing, Vector3 direction)
        {
            return direction;
        }
    }
}