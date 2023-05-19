using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("Constants/None")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.TextLight)]
    [Description("A vector with the constant (0, 0, 0)")]

    [Serializable]
    public class GetDirectionConstantZero : PropertyTypeGetDirection
    {
        public override Vector3 Get(Args args) => Vector3.back;

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionConstantZero()
        );

        public override string String => "None";
    }
}