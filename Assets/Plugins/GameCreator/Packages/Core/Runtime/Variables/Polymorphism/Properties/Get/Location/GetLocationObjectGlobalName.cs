using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Game Object Global Name Variable")]
    [Category("Variables/Game Object Global Name Variable")]
    
    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple, typeof(OverlayDot))]
    [Description("Returns the Game Object value of a Global Name Variable")]

    [Serializable]
    public class GetLocationObjectGlobalName : PropertyTypeGetLocation
    {
        [SerializeField]
        protected FieldGetGlobalName m_Variable = new FieldGetGlobalName(ValueGameObject.TYPE_ID);

        [SerializeField] private bool m_Rotate = true;
        
        public override Location Get(Args args)
        {
            GameObject value = this.m_Variable.Get<GameObject>(args);
            
            return new Location(
                value != null ? value.transform : null,
                Space.Self, Vector3.zero,
                this.m_Rotate,
                Quaternion.identity
            );
        }

        public override string String => this.m_Variable.ToString();
    }
}