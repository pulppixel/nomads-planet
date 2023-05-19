using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class EnablerLayerMask : TEnablerValue<LayerMask>
    {
        public EnablerLayerMask() : base(false, -1)
        { }
        
        public EnablerLayerMask(bool isEnabled) : base(isEnabled, -1)
        { }

        public EnablerLayerMask(LayerMask value) : base(false, value)
        { }
        
        public EnablerLayerMask(bool isEnabled, LayerMask value) : base(isEnabled, value)
        { }
    }
}