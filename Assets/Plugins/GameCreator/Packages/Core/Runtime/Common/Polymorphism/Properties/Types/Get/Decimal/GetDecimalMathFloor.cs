using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Floor Decimal")]
    [Category("Math/Floor Decimal")]
    
    [Image(typeof(IconNumber), ColorTheme.Type.TextNormal, typeof(OverlayArrowDown))]
    [Description("The current integer part of a decimal value")]

    [Keywords("Float", "Decimal", "Double")]
    
    [Serializable] [HideLabelsInEditor]
    public class GetDecimalMathFloor : PropertyTypeGetDecimal
    {
        [SerializeField] protected PropertyGetDecimal m_Number = new PropertyGetDecimal();

        public override double Get(Args args) => Math.Floor(this.m_Number.Get(args));
        public override double Get(GameObject gameObject) => Math.Floor(this.m_Number.Get(gameObject));

        public override string String => $"Floor {this.m_Number}";
    }
}