using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Game Object Local List Variable")]
    [Category("Variables/Game Object Local List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]
    [Description("Returns the Game Object value of a Local List Variable")]

    [Serializable]
    public class GetLocationObjectLocalList : PropertyTypeGetLocation
    {
        [SerializeField]
        protected FieldGetLocalList m_Variable = new FieldGetLocalList(ValueGameObject.TYPE_ID);

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