using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Game Object Local Name Variable")]
    [Category("Variables/Game Object Local Name Variable")]

    [Image(typeof(IconNameVariable), ColorTheme.Type.Purple)]
    [Description("Returns the Game Object value of a Local Name Variable")]
    
    [Serializable]
    public class GetLocationObjectLocalName : PropertyTypeGetLocation
    {
        [SerializeField]
        protected FieldGetLocalName m_Variable = new FieldGetLocalName(ValueGameObject.TYPE_ID);

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