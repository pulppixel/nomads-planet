using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Ceil Decimal")]
    [Category("Math/Ceil Decimal")]
    
    [Image(typeof(IconNumber), ColorTheme.Type.TextNormal, typeof(OverlayArrowUp))]
    [Description("The next integer part of a decimal value")]

    [Keywords("Float", "Decimal", "Double")]
    
    [Serializable] [HideLabelsInEditor]
    public class GetDecimalMathCeil : PropertyTypeGetDecimal
    {
        [SerializeField] protected PropertyGetDecimal m_Number = new PropertyGetDecimal();

        public override double Get(Args args) => Math.Ceiling(this.m_Number.Get(args));
        public override double Get(GameObject gameObject) => Math.Ceiling(this.m_Number.Get(gameObject));

        public override string String => $"Ceil {this.m_Number}";
    }
}