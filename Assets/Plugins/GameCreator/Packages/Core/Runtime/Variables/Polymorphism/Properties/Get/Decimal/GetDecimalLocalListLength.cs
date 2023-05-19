using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Variables
{
    [Title("Count of Local List Variable")]
    [Category("Variables/Count of Local List Variable")]
    
    [Image(typeof(IconListVariable), ColorTheme.Type.Teal)]
    [Description("Returns the amount of elements of a Local List Variable")]

    [Serializable] [HideLabelsInEditor]
    public class GetDecimalLocalListLength : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected LocalListVariables m_Variable;

        public override double Get(Args args) => this.m_Variable != null 
            ? this.m_Variable.Count
            : 0;
        
        public override double Get(GameObject gameObject) => this.m_Variable != null 
            ? this.m_Variable.Count
            : 0;

        public override string String => this.m_Variable != null
            ? this.m_Variable.gameObject.name + " Length"
            : "(none)";
    }
}